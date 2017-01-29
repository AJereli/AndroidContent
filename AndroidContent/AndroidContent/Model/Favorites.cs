using System;
using System.Collections.Generic;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Threading.Tasks;

namespace AllContent_Client
{
    class Favorit
    {
        public string Source { get; private set; }
        public List<ContentUnit> content;
        public uint SelectLimit { get; set; }
        private uint CurrId;
        private uint currDownload = 0;

        private List<string> selectResult = null;

        private MySqlParameter sqlParamSource;

        public Favorit(string source, uint selectLimit = 10)
        {
            SelectLimit = selectLimit;
            Source = source;
            content = new List<ContentUnit>();
            sqlParamSource = new MySqlParameter("sour", source);
        }
        /// <summary>
        /// Download first N item from mysql, where N - limit of download
        /// </summary>
        public void LoadAll()
        {
            using (DBClient client = new DBClient())
            {
                CurrId = Convert.ToUInt32(client.SelectQuery("SELECT COUNT(header) FROM content " +
                       $"WHERE source = @{sqlParamSource.ParameterName}", sqlParamSource)[0]);

                selectResult = client.SelectQuery("SELECT id, header, description, imgUrl, URL, tags, source, date FROM content " +
                            "WHERE source=@sour" + " ORDER BY localID DESC LIMIT " + SelectLimit
                            , sqlParamSource);
                AddToCUList();
            }
        }
        /// <summary>
        /// Download new news and returns true, if there is no new news - return false
        /// </summary>
        /// <param name="DownloadLimit"></param>
        /// <returns></returns>
        public bool LoadNextNews(uint DownloadLimit)
        {
            if (currDownload < CurrId)
            {
                using (DBClient client = new DBClient())
                {
                    selectResult = client.SelectQuery("SELECT id, header, description, imgUrl, URL, tags, source, date FROM content " +
                               "WHERE source = @" + sqlParamSource.ParameterName + " AND localID < " + (CurrId - currDownload).ToString() +
                               " ORDER BY localID DESC LIMIT " + DownloadLimit, sqlParamSource);
                    AddToCUList();
                }
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Downloading new items if they exist
        /// </summary>
        public void Refresh()
        {
            using (DBClient client = new DBClient())
            {
                List<string> chek_id = client.SelectQuery("SELECT COUNT(header) FROM content " +
                        $"WHERE source = @{sqlParamSource.ParameterName}", sqlParamSource);

                if (Convert.ToUInt32(chek_id[0]) > CurrId)
                {
                    selectResult = client.SelectQuery("SELECT id, header, description, imgUrl, URL, tags, source, date FROM content " +
                           $"WHERE source = @{sqlParamSource.ParameterName} AND localID > {chek_id.ToString()}" +
                            " ORDER BY localID DESC LIMIT " + SelectLimit, sqlParamSource);
                    AddToCUList();
                }
                CurrId = Convert.ToUInt32(chek_id[0]);
            }
        }

        public void DeleteContent()
        {
            content.Clear();
            currDownload = 0;
        }
        private void AddToCUList()
        {


            if (selectResult != null && selectResult.Count != 0)
            {
                content.Clear();
                //CurrId = (uint)Convert.ToUInt32(selectResult[0]);

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
                currDownload += (uint)content.Count;
            }
        }

    }


    class FavoritList : IEnumerable<Favorit>
    {
        public event Action<Favorit> AddEvent;
        public event Action<Favorit> ReloadAllhEvent;
        public event Action DeleteEvent = delegate { };
        public uint DownloadLimit { get; set; }


        private List<Favorit> favorites;
        static private FavoritList favorlist;

        private FavoritList()
        {
            DownloadLimit = 10;
            favorites = new List<Favorit>();
         
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

        

        public Task ReloadAll(AndroidContent.ItemAdapter adapter)
        {
            return Task.Run(() =>
            {
                foreach (var favor in favorites)
                {
                    favor.DeleteContent();
                    favor.LoadAll();
                    ReloadAllhEvent(favor);
                   // adapter.NotifyDataSetChanged();
                }
            });

        }

        public void LoadNextNews(AndroidContent.ItemAdapter adapter)
        {
            foreach (var favor in favorites)
            {
                if (favor.LoadNextNews(DownloadLimit))
                {
                    AddEvent(favor);
                    adapter.NotifyDataSetChanged();
                }
            }
        }



        public void Add(string source_name)
        {
            Favorit favor = new Favorit(source_name);
            favor.SelectLimit = DownloadLimit;
            favor.LoadAll();
            favorites.Add(favor);

            AddEvent(favor);
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