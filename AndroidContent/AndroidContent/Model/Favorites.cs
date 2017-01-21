using System;
using System.Collections.Generic;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections;

namespace AllContent_Client
{
    class Favorit
    {
        public string Source { get; private set; }
        public List<ContentUnit> content;
        public uint SelectLimit { get; set; }
        private uint CurrId;

        private List<string> selectResult = null;

        private MySqlParameter sqlParamSource;

        public Favorit(string source, uint selectLimit = 10)
        {
            SelectLimit = selectLimit;
            Source = source;
            content = new List<ContentUnit>();
            sqlParamSource = new MySqlParameter("sour", source);
        }

        public void LoadAll()
        {
            using (DBClient client = new DBClient())
            {
                selectResult = client.SelectQuery("SELECT id, header, description, imgUrl, URL, tags, source, date FROM content " +
                            "WHERE source=@sour" + " ORDER BY id DESC LIMIT " + SelectLimit
                            , sqlParamSource);
                AddToCUList();
            }
        }

        public void LoadNextNews(uint DownloadLimit)
        {
            using (DBClient client = new DBClient())
            {
                selectResult = client.SelectQuery("SELECT id, header, description, imgUrl, URL, tags, source, date FROM content " +
                           "WHERE source = @" + sqlParamSource.ParameterName + " AND id > " + CurrId.ToString() +
                           " ORDER BY id DESC LIMIT " + DownloadLimit, sqlParamSource);
                AddToCUList();
            }
        }

        public void Refresh()
        {
            using (DBClient client = new DBClient())
            {
                List<string> chek_id = client.SelectQuery("SELECT MAX(id) FROM content " +
                          "WHERE source = @" + sqlParamSource.ParameterName, sqlParamSource);


                if (Convert.ToUInt32(chek_id[0]) > CurrId)
                {
                    selectResult = client.SelectQuery("SELECT id, header, description, imgUrl, URL, tags, source, date FROM content " +
                            "WHERE source = @" + sqlParamSource.ParameterName + " AND id > " + CurrId.ToString() +
                            " ORDER BY id DESC LIMIT " + SelectLimit, sqlParamSource);
                    AddToCUList();
                }
            }
        }


        private void AddToCUList()
        {
            if (selectResult != null && selectResult.Count != 0)
            {
                CurrId = (uint)Convert.ToUInt32(selectResult[0]);

                for (int i = 0; i < selectResult.Count; i += 8)
                {
                    ContentUnit cu = new ContentUnit();

                    cu.ID = Convert.ToUInt32(selectResult[i]);
                    cu.header = selectResult[i + 1];
                    cu.description = selectResult[i + 2];
                    cu.imgUrl = selectResult[i + 3];
                    cu.URL = selectResult[i + 4];
                    cu.tags = selectResult[i + 5];
                    cu.source = selectResult[i + 6];
                    cu.date = selectResult[i + 7];
                    content.Add(cu);
                }
            }
        }

    }


    class FavoritList : IEnumerable<Favorit>
    {
        public event Action AddEvent = delegate { };

        public event Action DeleteEvent = delegate { };
        public uint  DownloadLimit { get; set; }


        private List<Favorit> favorites;
        private BackgroundWorker refreshAllContent;

        static private FavoritList favorlist;

        private FavoritList()
        {
            DownloadLimit = 10;
            favorites = new List<Favorit>();
            refreshAllContent = new BackgroundWorker();
            refreshAllContent.DoWork += RefreshAllContent_DoWork;
        }

        static public FavoritList Favorits
        {
            get
            {
                if (favorlist == null)
                    favorlist = new FavoritList();
                return favorlist;
            }
        }

        private void RefreshAllContent_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var favor in favorites)
                favor.Refresh();
            
        }


        public void RefreshAll()
        {
            refreshAllContent.RunWorkerAsync();
        }

        public void LoadNextNews()
        {
            foreach (var favor in favorites)
                favor.LoadNextNews(DownloadLimit);
            AddEvent();
        }

        public void Add(string source_name)
        {
            Favorit favor = new Favorit(source_name);
            favor.SelectLimit = DownloadLimit;
            favor.LoadAll();
            favorites.Add(favor);

            AddEvent();
        }
        public void Delete(string source_name)
        {
            foreach (var favor in favorites)
                if (favor.Source == source_name)
                {
                    favorites.Remove(favor);
                    break;
                }
            DeleteEvent();
        }

        public IEnumerator<Favorit> GetEnumerator()
        {
            return favorites.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return favorites.GetEnumerator();
        }
    }
}