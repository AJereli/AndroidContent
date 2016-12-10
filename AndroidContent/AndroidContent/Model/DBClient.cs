using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using System.Collections;

namespace AllContent_Client
{


    class DBClient : IDisposable
    {


        MySqlConnectionStringBuilder mysqlCSB;
        MySqlConnection mysqlConn;
        private object threadLock = new object();
        public DBClient()
        {
            mysqlConn = new MySqlConnection();
            mysqlCSB = new MySqlConnectionStringBuilder();
            mysqlCSB.Server = "194.87.239.28";
            mysqlCSB.Port = 3306;
            mysqlCSB.Database = "content";
            mysqlCSB.UserID = "user";
            mysqlCSB.Password = "TooEasyUserWork";
            mysqlConn.ConnectionString = mysqlCSB.ConnectionString;

        }
        public DBClient(MySqlConnectionStringBuilder _mysqlCSB)
        {
            mysqlConn = new MySqlConnection();
            mysqlCSB = _mysqlCSB;
            mysqlConn.ConnectionString = mysqlCSB.ConnectionString;

        }


        /// <summary>
        /// Implement INSERT, UPDATE or DELETE query
        /// </summary>
        /// <param name="query">Your SQL query</param>
        public void Query(string query, MySqlParameters parameters)
        {

            using (var mysqlConn = new MySqlConnection())
            {
                if (query.Contains("SELECT"))
                {
                    throw new Exception("WRONG TYPE OF SQL QUERY, NEED INSERT / UPDATE / DELETE");
                }
                lock (threadLock)
                {
                    mysqlConn.ConnectionString = mysqlCSB.ConnectionString;
                    mysqlConn.Open();
                    MySqlCommand com = new MySqlCommand(@query, mysqlConn);


                    foreach (var param in parameters)
                        com.Parameters.Add(param);


                    MySqlDataReader dataReader = com.ExecuteReader();
                    dataReader.Read();
                    dataReader.Close();
                }
            }
        }
        /// <summary>
        /// Same, but only one parameter
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameter"></param>
        public void Query(string query, MySqlParameter parameter)
        {
            MySqlParameters parameters = new MySqlParameters();
            parameters.AddParameter(parameter);
            Query(query, parameters);
        }
        /// <summary>
        /// Select information from DB
        /// </summary>
        /// <param name="query">Your SELECT query</param>
        /// <param name="parameters">All parameters</param>
        /// <returns>list of selected information</returns>

        public List<string> SelectQuery(string query, MySqlParameters parameters)
        {
            using (var mysqlConn = new MySqlConnection())
            {
                mysqlConn.ConnectionString = mysqlCSB.ConnectionString;

                mysqlConn.Open();
                
                if (query.Contains("INSERT INTO"))
                {
                    throw new Exception("WRONG TYPE OF SQL QUERY, NEED SELECT");
                }
                List<string> result = new List<string>();


                MySqlCommand com = new MySqlCommand(@query, mysqlConn);

                foreach (var param in parameters)
                    com.Parameters.Add(param);


                using (var dataReader = com.ExecuteReader())
                {


                    while (dataReader.Read())
                    {
                        try
                        { 
                            for (int i = 0; i < dataReader.FieldCount; ++i)
                                result.Add(dataReader.GetString(i));
                        }
                        catch (System.Data.SqlTypes.SqlNullValueException)
                        {
                            dataReader.Close();
                            return result;

                        }
                    }

                    dataReader.Close();

                    return result;

                }
            }
        }
        /// <summary>
        /// Same, but only one parameter
        /// </summary>

        public List<string> SelectQuery(string query, MySqlParameter parameter)
        {
            MySqlParameters parameters = new MySqlParameters();
            parameters.AddParameter(parameter);
            return SelectQuery(query, parameters);
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    mysqlConn.Dispose();
                   
                }

                mysqlCSB = null;

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~DBClient() {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
    class MySqlParameters : IEnumerable
    {
        private List<MySqlParameter> parameters { get; }
        public MySqlParameters()
        {
            parameters = new List<MySqlParameter>();
        }

        public void AddParameter(MySqlParameter param)
        {
            parameters.Add(param);
        }

        public void AddParameter(string param_name, object param_value)
        {
            parameters.Add(new MySqlParameter(param_name, param_value));
        }

        public IEnumerator GetEnumerator()
        {
            return parameters.GetEnumerator();
        }
    }
}
