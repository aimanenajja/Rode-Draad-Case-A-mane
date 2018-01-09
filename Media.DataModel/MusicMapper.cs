using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media.DataModel;
using Media.DataModel.Exceptions;

namespace Media.DataModel
{
    public class MusicMapper : IMapper
    {
        SqlConnection cn;

        public ObservableCollection<DataModel.Media> GetAllMedia()
        {
            ObservableCollection<DataModel.Media> songs = new ObservableCollection<DataModel.Media>();
            string SqlStatement = "SELECT Id, Title, Singer FROM Song";
            try {
                using (SqlConnection cn = MediaDB.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(SqlStatement, cn))
                    {
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            int idPos = dr.GetOrdinal("Id");
                            int titlePos = dr.GetOrdinal("Title");
                            int singerPos = dr.GetOrdinal("Singer");
                            Song song;
                            while (dr.Read())
                            {
                                song = new Song
                                {
                                    Id = (int)dr[idPos],
                                    Title = dr[titlePos].ToString(),
                                    Singer = dr[singerPos].ToString()
                                };
                                songs.Add(song);
                            }
                        }

                    }
                }
            }
            catch (SqlException e)
            {
                throw new MediaReadFailedException(e);
            }
            return songs;
        }

        public byte[] GetMediaFile(int songId)
        {
            byte[] musicFile = null;
            string SqlStatement  = "SELECT File FROM Song WHERE Id = @songId";
            try
            {
                using (SqlConnection cn = MediaDB.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(SqlStatement, cn))
                    {
                        cmd.Parameters.AddWithValue("@songId", songId);
                        cn.Open();
                        musicFile = (byte []) cmd.ExecuteScalar();
                    }
                }
            }
            catch (SqlException e)
            {
                throw new MediaReadFailedException(e);
            }
            return musicFile;

        }

        public DataModel.Media AddMedia(DataModel.Media newMedia)
        {
            string addQuery;

            if (newMedia.File == null)
            {
                addQuery = "INSERT INTO Song (Title, Singer) VALUES ('" + newMedia.Title + "', '" + ((Song)newMedia).Singer + "');"
                    + " SELECT CAST(scope_identity() AS int);";
            }
            else
            {
                addQuery = "INSERT INTO Song (Title, Singer, File) VALUES ('" + newMedia.Title + "', '" + ((Song)newMedia).Singer + "', '"
                    + "0x" + BitConverter.ToString(newMedia.File).Replace("-", "") + "');"
                    + " SELECT CAST(scope_identity() AS int);";
            }

            try
            {
                using (SqlConnection cn = MediaDB.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(addQuery, cn))
                    {
                        cn.Open();
                        newMedia.Id = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (SqlException)
            {
                throw new SaveMediaFailedException();
            }
            return newMedia;
        }

        public bool UpdateMedia(DataModel.Media updateMedia)
        {
            int updateCount = 0;
            string updateQuery;

            if (updateMedia.File == null)
            {
                updateQuery = "UPDATE Song SET Title = '" + updateMedia.Title + "', Singer = '" + ((Song)updateMedia).Singer + "' WHERE Id = " + updateMedia.Id;
            }
            else
            {
                updateQuery = "UPDATE Song SET Title = '" + updateMedia.Title + "', Singer = '" + ((Song)updateMedia).Singer + "', File = '" 
                    + "0x" + BitConverter.ToString(updateMedia.File).Replace("-", "")  
                    + "' WHERE Id = " + updateMedia.Id;
            }

            try
            {
                using (SqlConnection cn = MediaDB.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(updateQuery, cn))
                    {
                        cn.Open();
                        updateCount = cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (SqlException)
            {
                throw new UpdateMediaFailedException();
            }
            return updateCount > 0;
        }

        public bool DeleteMedia(DataModel.Media oldMedia)
        {
            int deleteCount = 0;
            string deleteQuery = "DELETE FROM Song WHERE Id = " + oldMedia.Id;

            try
            { 
                using (SqlConnection cn = MediaDB.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, cn))
                    {
                        cn.Open();
                        deleteCount = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException)
            {
                throw new RemoveMediaFailedException();
            }
            return deleteCount > 0;
        }
    }
}
