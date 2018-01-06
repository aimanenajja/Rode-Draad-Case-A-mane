using Media.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Controller.Ex01
{
    /// <summary>
    /// A controller for media items of the type Movie.
    /// </summary>
    public class MovieController : MediaController
    {
        /// <summary>
        /// Constructor that calls the base constructor without further actions.
        /// </summary>
        public MovieController() : base()
        {

        }

        /// <summary>
        /// Builds a string that can be used in an OpenFileDialog as a filter for the type of files (extensions) you want to load.
        /// </summary>
        /// <returns>A string that can be passed to an OpenFileDialog as filter for the type of files you can add.</returns>
        public override string FileFilter()
        {
            return "Video Files(*.avi) | *.avi;";
        }

        /// <summary>
        /// Loads data in the memory (List).
        /// </summary>
        protected override void InitializeData()
        {
            List = new ObservableCollection<DataModel.Media>
            {   new Movie()
                {
                    Title = "The Jungle Book",
                    Director = "Jon Favreau"
                },
                new Movie()
                {
                    Title = "Data Binding",
                    Director = "Test"
                }
            };
        }
    }
}
