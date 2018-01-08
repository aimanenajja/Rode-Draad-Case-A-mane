using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media.DataModel;

namespace Media.Mapper
{
    public class MovieMapper : IMapper
    {


        public ObservableCollection<DataModel.Media> GetAllMedia => throw new NotImplementedException();

        public byte[] GetMediaFile => throw new NotImplementedException();

        public void AddMedia(DataModel.Media newMedia)
        {
            throw new NotImplementedException();
        }

        public void DeleteMedia(DataModel.Media oldMedia)
        {
            throw new NotImplementedException();
        }

        public void UpdateMedia(DataModel.Media oldMedia)
        {
            throw new NotImplementedException();
        }
    }
}
