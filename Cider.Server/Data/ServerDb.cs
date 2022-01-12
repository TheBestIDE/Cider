using Microsoft.Data.Sqlite;
using Cider.IO;
using Cider.Server.Data.Model;

namespace Cider.Server.Data
{
    public class ServerDb : IServerDb, IDisposable
    {
        protected SqliteConnection sqlConn;

        public static string DbFileName { get; } = "CiderServer.sqlite";

        public static string DbFileDirectory { get; } = "db";

        protected static string DbPassword { get; } = "Cider123456";

        static ServerDb()
        {
            InitDbFile();
            var sqlConn = CreateConnection();
            CreateTables(sqlConn);
        }

        /// <summary>新建数据库处理实例</summary>
        public ServerDb()
        {
            sqlConn = CreateConnection();
            OpenConnection(sqlConn);
        }

        #pragma warning disable 8602
        public Files? GetFile(string fileName)
        {
            string fSql = @"SELECT * FROM Files WHERE FileName = @FileName;";

            using var sqlcmd = new SqliteCommand(fSql, sqlConn);
            sqlcmd.Parameters.Add(new SqliteParameter("@FileName", fileName));
            using var result = sqlcmd.ExecuteReader();
            Files? file = result.HasRows 
                            ? new Files() 
                            { 
                                FileName = fileName,
                                BlockHash = new List<string>() 
                            } 
                            : null;
            while (result.Read())
            {
                file.BlockHash.Add(result.GetString(2));
            }

            return file;
        }

        public void InsertFile(Files fb)
        {
            string fSql = @"INSERT INTO Files 
                                (FileName, BlockHash)
                            VALUES
                                (@FileName, @BlockHash)";

            var transaction = sqlConn.BeginTransaction();

            using var sqlcmd = sqlConn.CreateCommand();
            sqlcmd.CommandText = fSql;

            var param1 = new SqliteParameter("@FileName", fb.FileName);
            var param2 = new SqliteParameter()
            {
                ParameterName = "@BlockHash"
            };
            sqlcmd.Parameters.Add(param1);
            sqlcmd.Parameters.Add(param2);

            foreach(var hash in fb.BlockHash)
            {
                param2.Value = hash;
                sqlcmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        public void DeleteFile(string fileName)
        {
            string fSql = @"DELETE FROM Files WHERE FileName = @FileName";

            using var sqlcmd = sqlConn.CreateCommand();
            sqlcmd.CommandText = fSql;
            sqlcmd.Parameters.Add(new SqliteParameter("@FileName", fileName));
            sqlcmd.ExecuteNonQuery();
        }

        public FileBlocks? GetBlock(string hash)
        {
            string fbSql = @"SELECT * FROM FileBlocks WHERE BlockHash = @BlockHash";

            using var sqlcmd = new SqliteCommand(fbSql, sqlConn);
            sqlcmd.Parameters.Add(new SqliteParameter("@BlockHash", hash));
            using var result = sqlcmd.ExecuteReader();
            FileBlocks? fb = result.Read()
                                ? new FileBlocks()
                                {
                                    BlockHash = hash,
                                    BlockFileName = result.GetString(1)
                                }
                                : null;
            return fb;
        }

        public void InsertBlock(FileBlocks fb)
        {
            string fbSql = @"INSERT INTO FileBlocks
                                (BlockHash, BlockFileName)
                            VALUES
                                (@BlockHash, @BlockFileName)";
            
            using var sqlcmd = new SqliteCommand(fbSql, sqlConn);
            sqlcmd.Parameters.Add(new SqliteParameter("@BlockHash", fb.BlockHash));
            sqlcmd.Parameters.Add(new SqliteParameter("@BlockFileName", fb.BlockFileName));
            sqlcmd.ExecuteNonQuery();
        }

        public void InsertBlocks(IEnumerable<FileBlocks> fbs)
        {
            string fbSql = @"INSERT INTO FileBlocks
                                (BlockHash, BlockFileName)
                            VALUES
                                (@BlockHash, @BlockFileName)";

            // 开启事务
            using var transaction = sqlConn.BeginTransaction();
            using var sqlcmd = sqlConn.CreateCommand();
            sqlcmd.CommandText = fbSql;
            // 创建参数
            var param1 = new SqliteParameter()
            {
                ParameterName = "@BlockHash"
            };
            var param2 = new SqliteParameter()
            {
                ParameterName = "@BlockFileName"
            };
            sqlcmd.Parameters.Add(param1);
            sqlcmd.Parameters.Add(param2);
            // 写入数据
            foreach (var fb in fbs)
            {
                param1.Value = fb.BlockHash;
                param2.Value = fb.BlockFileName;
                sqlcmd.ExecuteNonQuery();
            }
            //提交事务
            transaction.Commit();
        }

        public void DeleteBlock(string hash)
        {
            string fbSql = @"DELETE FROM FileBlocks WHERE BlockHash = @BlockHash";

            using var sqlcmd = new SqliteCommand(fbSql, sqlConn);
            sqlcmd.Parameters.Add(new SqliteParameter("@BlockHash", hash));
            sqlcmd.ExecuteNonQuery();
        }

        public void DeleteBlocks(IEnumerable<string> hashs)
        {
            string fbSql = @"DELETE FROM FileBlocks WHERE BlockHash = @BlockHash";

            using var transaction = sqlConn.BeginTransaction();
            using var sqlcmd = sqlConn.CreateCommand();
            sqlcmd.CommandText = fbSql;

            var param = new SqliteParameter()
            {
                ParameterName = "@BlockHash"
            };
            sqlcmd.Parameters.Add(param);

            foreach (var hash in hashs)
            {
                param.Value = hash;
                sqlcmd.ExecuteNonQuery();
            }
            transaction.Commit();
        }

        public string[]? CombineFilesAndBlocks(string fileName)
        {
            string sql = @" SELECT 
                                FileBlocks.BlockFileName 
                            FROM 
                                (SELECT BlockHash FROM Files WHERE FileName = @FileName) f
                            JOIN 
                                FileBlocks
                            ON f.BlockHash = FileBlocks.BlockHash";
            using var sqlcmd = new SqliteCommand(sql, sqlConn);
            sqlcmd.Parameters.Add(new SqliteParameter("@FileName", fileName));
            using var result = sqlcmd.ExecuteReader();
            List<string> li = new List<string>();
            while (result.Read())
            {
                li.Add(result.GetString(0));
            }
            return li.Count == 0 ? null : li.ToArray();
        }

        public void Dispose()
        {
            CloseConnection(sqlConn);
        }

        /// <summary>打开连接</summary>
        public static void OpenConnection(SqliteConnection sqlConn)
        {
            if (sqlConn.State != System.Data.ConnectionState.Open)
                sqlConn.Open();
        }

        public static bool IsOpen(SqliteConnection sqlConn)
        {
            return sqlConn.State == System.Data.ConnectionState.Open;
        }

        /// <summary>关闭连接</summary>
        public static void CloseConnection(SqliteConnection sqlConn)
        {
            if (sqlConn.State != System.Data.ConnectionState.Closed)
                sqlConn.Close();
        }

        protected static void InitDbFile()
        {
            string dbFile = "./" + DbFileDirectory + "/" + DbFileName;
            if (!Directory.Exists("./" + DbFileDirectory))
                Directory.CreateDirectory("./" + DbFileDirectory);
            if (!File.Exists(dbFile))
                File.Create(dbFile).Dispose();
        }

        /// <summary>创建连接</summary>
        protected static SqliteConnection CreateConnection()
        {
            string connString = new SqliteConnectionStringBuilder()
            {
                DataSource = "./" + DbFileDirectory + "/" + DbFileName,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Shared,
                Password = DbPassword
            }.ToString();
            return new SqliteConnection(connString);
        }

        /// <summary>创建表</summary>
        protected static void CreateTables(SqliteConnection sqlConn)
        {
            // 文件信息表
            var ftSql = @"
                        CREATE TABLE IF NOT EXISTS Files (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                            FileName TEXT NOT NULL,
                            BlockHash TEXT NOT NULL
                        );";
            // 文件块表
            var fbtSql = @"
                        CREATE TABLE IF NOT EXISTS FileBlocks (
                            BlockHash TEXT NOT NULL, 
                            BlockFileName TEXT NOT NULL,
                            PRIMARY KEY (BlockHash)
                        );";
            // 创建表
            OpenConnection(sqlConn);
            using var sqlcmd = new SqliteCommand(ftSql, sqlConn);
            sqlcmd.ExecuteNonQuery();
            sqlcmd.CommandText = fbtSql;
            sqlcmd.ExecuteNonQuery();
            CloseConnection(sqlConn);
        }
    }
}