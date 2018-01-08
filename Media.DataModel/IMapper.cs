using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media.DataModel;

namespace Media.Mapper
{
    public interface IMapper
    {
        ObservableCollection<DataModel.Media> GetAllMedia();
        byte[] GetMediaFile(int id);
        void AddMedia(DataModel.Media newMedia);
        bool UpdateMedia(DataModel.Media updateMedia);
        bool DeleteMedia(DataModel.Media oldMedia); 
    }
}
