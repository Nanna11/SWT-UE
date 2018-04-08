using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using BIF.SWE2.Interfaces;
using BIF.SWE2.Interfaces.Models;
using BIF.SWE2.Interfaces.ViewModels;
using PicDB.Models;
using PicDB.ViewModels;
using PicDB;
using System.ComponentModel;
using System.Windows.Input;

namespace BIF.SWE2.UnitTests
{
    [TestFixture]
    public class UnitTests
    {
        [Test]
        public void PictureViewModel_should_return_DisplayName_without_IPTC_Headline_set()
        {
            string Filename = "Testfile.jpg";
            string LastName = "LastName";
            string Expected = Filename + " (by " + LastName + ")";

            PictureModel pictureModel = new PictureModel(Filename);

            IPTCModel IPTCModel = new IPTCModel();
            IPTCModel.Headline = null;
            pictureModel.IPTC = IPTCModel;

            PhotographerModel photographer = new PhotographerModel();
            photographer.LastName = LastName;
            pictureModel.Photographer = photographer;

            PictureViewModel pictureViewModel = new PictureViewModel(pictureModel);

            Assert.IsTrue(pictureViewModel.DisplayName == Expected);
        }

        [Test]
        public void DBFactory_should_return_correct_DAL()
        {
            DBConnectionFactory dbf = DBConnectionFactory.Instance;
            dbf.Mock = true;

            IDataAccessLayer dal1 = dbf.CreateDal("","","","");

            dbf.Mock = false;

            IDataAccessLayer dal2 = null;
            try
            {
                dal2 = dbf.CreateDal("", "", "", "");
            }
            catch (Exception) { }

            Assert.AreNotEqual(dal1.GetType(), dal2.GetType());
            
        }

        [Test]
        public void PictureViewModel_should_return_DisplayName_without_Photographer_set()
        {
            string Filename = "Testfile.jpg";
            string Headline = "Testfile";
            string ByLine = "LastName";
            string Expected = Headline + " (by " + ByLine + ")";

            PictureModel pictureModel = new PictureModel(Filename);

            IPTCModel IPTCModel = new IPTCModel();
            IPTCModel.Headline = Headline;
            IPTCModel.ByLine = ByLine;
            pictureModel.IPTC = IPTCModel;

            pictureModel.Photographer = null;

            PictureViewModel pictureViewModel = new PictureViewModel(pictureModel);

            Assert.IsTrue(pictureViewModel.DisplayName == Expected);
        }

        [Test]
        public void BussinesLayer_should_save_Camera()
        {
            //DBConnectionFactory dbf = DBConnectionFactory.Instance;
            //dbf.Mock = true;
            DataAccessLayerFactory dalFactory = DataAccessLayerFactory.Instance;
            dalFactory.IsReal = false;

            string path = "path";

            BusinessLayer businessLayer = new BusinessLayer(path);

            CameraModel camera = new CameraModel();

            int OldCount = businessLayer.GetCameras().Count();
            businessLayer.Save(camera);
            int NewCount = businessLayer.GetCameras().Count();

            Assert.IsTrue(OldCount == NewCount - 1);
            Assert.IsTrue(businessLayer.GetCameras().Contains(camera));
        }

        [Test]
        public void BussinesLayer_should_delete_Camera()
        {
            //DBConnectionFactory dbf = DBConnectionFactory.Instance;
            //dbf.Mock = true;
            DataAccessLayerFactory dalFactory = DataAccessLayerFactory.Instance;
            dalFactory.IsReal = false;

            string path = "path";

            BusinessLayer businessLayer = new BusinessLayer(path);

            CameraModel camera = new CameraModel();

            businessLayer.Save(camera);
            int OldCount = businessLayer.GetCameras().Count();

            Assert.GreaterOrEqual(OldCount, 1);

            List<ICameraModel> CameraList = businessLayer.GetCameras().ToList<ICameraModel>();
            CameraModel Cameramodel = (CameraModel)CameraList[0];

            businessLayer.DeleteCamera(Cameramodel.ID);

            Assert.IsTrue(OldCount > businessLayer.GetCameras().Count());
            Assert.IsFalse(businessLayer.GetCameras().Contains(Cameramodel));
        }

        [Test]
        public void PictureListViewModel_should_return_previous_Pictures()
        {
            //DBConnectionFactory dbf = DBConnectionFactory.Instance;
            //dbf.Mock = true;
            DataAccessLayerFactory dalFactory = DataAccessLayerFactory.Instance;
            dalFactory.IsReal = false;

            string path = "path";
            int currentIndex = 3;
            int pictureCount = 5;

            BusinessLayer businessLayer = new BusinessLayer(path);
            int ExistingPictureCount = businessLayer.GetPictures().Count();
            currentIndex += businessLayer.GetPictures().Count();

            for (int i = 0; i < pictureCount; i++)
            {
                businessLayer.Save(new PictureModel("test " + i.ToString()));
            }

            pictureCount += ExistingPictureCount;

            List<IPictureModel> pictureList = businessLayer.GetPictures().ToList<IPictureModel>();
            List<IPictureViewModel> pictureViewList = new List<IPictureViewModel>();

            foreach (IPictureModel pm in pictureList)
            {
                pictureViewList.Add(new PictureViewModel(pm));
            }

            PictureListViewModel pictureListViewModel = new PictureListViewModel(pictureViewList);
            pictureListViewModel.CurrentIndex = currentIndex;

            Assert.IsTrue(pictureViewList[currentIndex] == pictureListViewModel.CurrentPicture);

            Assert.IsTrue(pictureListViewModel.PrevPictures.Count<IPictureViewModel>() == currentIndex);
            Assert.IsFalse(pictureListViewModel.PrevPictures.Contains(pictureListViewModel.CurrentPicture));
            for (int i = 0; i < pictureCount; i++)
            {
                if (i < currentIndex) Assert.IsTrue(pictureListViewModel.PrevPictures.Contains(pictureViewList[i]));
                else Assert.IsFalse(pictureListViewModel.PrevPictures.Contains(pictureViewList[i]));
            }

        }

        [Test]
        public void PictureListViewModel_should_return_next_Pictures()
        {
            //DBConnectionFactory dbf = DBConnectionFactory.Instance;
            //dbf.Mock = true;
            DataAccessLayerFactory dalFactory = DataAccessLayerFactory.Instance;
            dalFactory.IsReal = false;

            string path = "path";
            int currentIndex = 3;
            int pictureCount = 5;

            BusinessLayer businessLayer = new BusinessLayer(path);
            int ExistingPictureCount = businessLayer.GetPictures().Count();
            currentIndex += businessLayer.GetPictures().Count();

            for (int i = 0; i < pictureCount; i++)
            {
                businessLayer.Save(new PictureModel("test " + i.ToString()));
            }

            pictureCount += ExistingPictureCount;

            List<IPictureModel> pictureList = businessLayer.GetPictures().ToList<IPictureModel>();
            List<IPictureViewModel> pictureViewList = new List<IPictureViewModel>();

            foreach (IPictureModel pm in pictureList)
            {
                pictureViewList.Add(new PictureViewModel(pm));
            }

            PictureListViewModel pictureListViewModel = new PictureListViewModel(pictureViewList);
            pictureListViewModel.CurrentIndex = currentIndex;

            Assert.IsTrue(pictureViewList[currentIndex] == pictureListViewModel.CurrentPicture);

            Assert.IsTrue(pictureListViewModel.NextPictures.Count<IPictureViewModel>() == pictureCount - currentIndex - 1);
            Assert.IsFalse(pictureListViewModel.NextPictures.Contains(pictureListViewModel.CurrentPicture));

            for (int i = 0; i < pictureCount; i++)
            {
                if (i > currentIndex) Assert.IsTrue(pictureListViewModel.NextPictures.Contains(pictureViewList[i]));
                else Assert.IsFalse(pictureListViewModel.NextPictures.Contains(pictureViewList[i]));
            }
        }

        [Test]
        public void PictureListViewModel_should_represent_Current_Picture_as_String()
        {
            //DBConnectionFactory dbf = DBConnectionFactory.Instance;
            //dbf.Mock = true;
            DataAccessLayerFactory dalFactory = DataAccessLayerFactory.Instance;
            dalFactory.IsReal = false;

            string path = "path";
            string Expected;
            int currentIndex = 3;
            int pictureCount = 5;

            BusinessLayer businessLayer = new BusinessLayer(path);
            int ExistingPictureCount = businessLayer.GetPictures().Count();
            currentIndex += businessLayer.GetPictures().Count();

            for (int i = 0; i < pictureCount; i++)
            {
                businessLayer.Save(new PictureModel("test " + i.ToString()));
            }

            pictureCount += ExistingPictureCount;

            Expected = (currentIndex + 1).ToString() + " out of " + pictureCount.ToString();

            List<IPictureModel> pictureList = businessLayer.GetPictures().ToList<IPictureModel>();

            PictureListViewModel pvm = new PictureListViewModel(pictureList);

            Assert.IsTrue(pvm.CurrentPictureAsString == Expected);
        }

        [Test]
        public void PictureListViewModel_Current_Picture_should_match_Current_Index()
        {
            //DBConnectionFactory dbf = DBConnectionFactory.Instance;
            //dbf.Mock = true;
            DataAccessLayerFactory dalFactory = DataAccessLayerFactory.Instance;
            dalFactory.IsReal = false;

            string path = "path";
            int currentIndex = 3;
            int pictureCount = 5;

            BusinessLayer businessLayer = new BusinessLayer(path);
            currentIndex += InsertPictureModels(pictureCount, businessLayer);

            List<IPictureModel> pictureList = businessLayer.GetPictures().ToList<IPictureModel>();
            List<IPictureViewModel> pictureViewList = PictureModelToPictureViewModel(pictureList);

            PictureListViewModel pictureListViewModel = new PictureListViewModel(pictureViewList);
            pictureListViewModel.CurrentIndex = currentIndex;

            Assert.IsTrue(pictureViewList[currentIndex] == pictureListViewModel.CurrentPicture);

        }

        //[Test]
        //public void PhotographerViewModel_should_return_number_of_pictures()
        //{
        //    int Expected = 2;

        //    DBConnectionFactory dbf = DBConnectionFactory.Instance;
        //    dbf.Mock = true;

        //    string path = "path";

        //    BusinessLayer businessLayer = new BusinessLayer(path);

        //    PhotorapherModel photographer = new PhotorapherModel();
        //    photographer.LastName = "Lastname";

        //    businessLayer.Save(photographer);

        //    InsertPictureModels(5, businessLayer);

        //    List<IPictureModel> pictures = businessLayer.GetPictures().ToList<IPictureModel>();
        //    photographer = (PhotorapherModel)businessLayer.GetPhotographers().ElementAt(0);
        //    ((PictureModel)pictures[0]).Photographer = photographer;
        //    ((PictureModel)pictures[1]).Photographer = photographer;
        //    businessLayer.Save(pictures[0]);
        //    businessLayer.Save(pictures[1]);

        //    photographer = (PhotorapherModel)businessLayer.GetPhotographer(photographer.ID);
        //}

        [Test]
        public void PictureModel_should_return_Photographer()
        {
            PhotorapherModel photographer = new PhotorapherModel();
            photographer.LastName = "Lastname";

            PictureModel picture = new PictureModel("test");
            picture.Photographer = photographer;

            Assert.NotNull(picture.Photographer);
            Assert.AreEqual(photographer, picture.Photographer);
        }

        [Test]
        public void PictureViewModel_should_return_Photographer()
        {
            string LastName = "Lastname";
            PhotorapherModel photographer = new PhotorapherModel();
            photographer.LastName = LastName;

            PictureModel picture = new PictureModel("test");
            picture.Photographer = photographer;

            PictureViewModel pvm = new PictureViewModel(picture);

            Assert.NotNull(pvm.Photographer);
            Assert.AreEqual(LastName, pvm.Photographer.LastName);
        }

        [Test]
        public void PictureModels_with_same_photographer_should_point_to_same_object()
        {

            DBConnectionFactory dbf = DBConnectionFactory.Instance;
            dbf.Mock = true;

            string path = "path";

            BusinessLayer businessLayer = new BusinessLayer(path);

            PhotorapherModel photographer = new PhotorapherModel();
            photographer.LastName = "Lastname";

            businessLayer.Save(photographer);

            InsertPictureModels(5, businessLayer);

            List<IPictureModel> pictures = businessLayer.GetPictures().ToList<IPictureModel>();
            photographer = (PhotorapherModel)businessLayer.GetPhotographers().ElementAt(0);
            ((PictureModel)pictures[0]).Photographer = photographer;
            ((PictureModel)pictures[1]).Photographer = photographer;
            businessLayer.Save(pictures[0]);
            businessLayer.Save(pictures[1]);

            pictures = businessLayer.GetPictures().ToList<IPictureModel>();
            Assert.AreEqual(((PictureModel)pictures[0]).Photographer, ((PictureModel)pictures[1]).Photographer);
        }

        [Test]
        public void PictureModels_with_same_camera_should_point_to_same_object()
        {

            DBConnectionFactory dbf = DBConnectionFactory.Instance;
            dbf.Mock = true;

            string path = "path";

            BusinessLayer businessLayer = new BusinessLayer(path);

            CameraModel cm = new CameraModel();

            businessLayer.Save(cm);

            InsertPictureModels(5, businessLayer);

            List<IPictureModel> pictures = businessLayer.GetPictures().ToList<IPictureModel>();
            cm = (CameraModel)businessLayer.GetCameras().ElementAt(0);
            ((PictureModel)pictures[0]).Camera = cm;
            ((PictureModel)pictures[1]).Camera = cm;
            businessLayer.Save(pictures[0]);
            businessLayer.Save(pictures[1]);

            pictures = businessLayer.GetPictures().ToList<IPictureModel>();
            Assert.AreEqual(((PictureModel)pictures[0]).Camera, ((PictureModel)pictures[1]).Camera);
        }

        [Test]
        public void PictureViewModel_should_notify_on_DisplayName_changed_when_headline_changed()
        {
            List<string> expectedEvents = new List<string>();
            expectedEvents.Add("DisplayName");
            List<string> receivedEvents = new List<string>();
            PictureModel picture = new PictureModel("test");
            picture.EXIF = GetDemoExif();
            picture.IPTC = GetDemoIPTC();

            PictureViewModel pvm = new PictureViewModel(picture);

            pvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                receivedEvents.Add(e.PropertyName);
            };

            pvm.IPTC.Headline = "test";

            foreach(string s in expectedEvents)
            {
                Assert.IsTrue(receivedEvents.Contains(s));
            }
        }

        [Test]
        public void PictureViewModel_should_notify_on_DisplayName_changed_when_ByLine_changed()
        {
            List<string> expectedEvents = new List<string>();
            expectedEvents.Add("DisplayName");
            List<string> receivedEvents = new List<string>();
            PictureModel picture = new PictureModel("test");
            picture.EXIF = GetDemoExif();
            picture.IPTC = GetDemoIPTC();

            PictureViewModel pvm = new PictureViewModel(picture);

            pvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                receivedEvents.Add(e.PropertyName);
            };

            pvm.IPTC.ByLine = "test";

            foreach (string s in expectedEvents)
            {
                Assert.IsTrue(receivedEvents.Contains(s));
            }
        }

        [Test]
        public void PictureViewModel_should_notify_on_DisplayName_changed_when_Photographer_changed()
        {
            List<string> expectedEvents = new List<string>();
            expectedEvents.Add("DisplayName");
            List<string> receivedEvents = new List<string>();
            PictureModel picture = new PictureModel("test");
            picture.EXIF = GetDemoExif();
            picture.IPTC = GetDemoIPTC();

            PictureViewModel pvm = new PictureViewModel(picture);

            pvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                receivedEvents.Add(e.PropertyName);
            };

            pvm.Photographer = new PhotographerViewModel(new PhotorapherModel());

            foreach (string s in expectedEvents)
            {
                Assert.IsTrue(receivedEvents.Contains(s));
            }
        }

        [Test]
        public void PictureViewModel_should_notify_on_Photographer_changed()
        {
            List<string> expectedEvents = new List<string>();
            expectedEvents.Add("Photographer");
            List<string> receivedEvents = new List<string>();
            PictureModel picture = new PictureModel("test");
            picture.EXIF = GetDemoExif();
            picture.IPTC = GetDemoIPTC();

            PictureViewModel pvm = new PictureViewModel(picture);

            pvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                receivedEvents.Add(e.PropertyName);
            };

            pvm.Photographer = new PhotographerViewModel(new PhotorapherModel());

            foreach (string s in expectedEvents)
            {
                Assert.IsTrue(receivedEvents.Contains(s));
            }
        }

        [Test]
        public void PictureViewModel_should_notify_on_Camera_changed()
        {
            List<string> expectedEvents = new List<string>();
            expectedEvents.Add("Camera");
            List<string> receivedEvents = new List<string>();
            PictureModel picture = new PictureModel("test");
            picture.EXIF = GetDemoExif();
            picture.IPTC = GetDemoIPTC();

            PictureViewModel pvm = new PictureViewModel(picture);

            pvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                receivedEvents.Add(e.PropertyName);
            };

            pvm.Camera = new CameraViewModel(new CameraModel());

            foreach (string s in expectedEvents)
            {
                Assert.IsTrue(receivedEvents.Contains(s));
            }
        }

        [Test]
        public void PictureViewModel_should_notify_on_Camera_changed_when_EXIF_is_changed()
        {
            List<string> expectedEvents = new List<string>();
            expectedEvents.Add("Camera");
            List<string> receivedEvents = new List<string>();
            PictureModel picture = new PictureModel("test");
            picture.EXIF = GetDemoExif();
            picture.IPTC = GetDemoIPTC();

            PictureViewModel pvm = new PictureViewModel(picture);

            pvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                receivedEvents.Add(e.PropertyName);
            };

            pvm.EXIF = new EXIFViewModel(new EXIFModel());

            foreach (string s in expectedEvents)
            {
                Assert.IsTrue(receivedEvents.Contains(s));
            }
        }

        [Test]
        public void EXIFViewModel_should_notify_on_ISORating_changed_when_Camera_changed()
        {
            List<string> expectedEvents = new List<string>();
            expectedEvents.Add("ISORating");
            List<string> receivedEvents = new List<string>();
            IEXIFModel exif = GetDemoExif();
            EXIFViewModel evm = new EXIFViewModel(exif);

            evm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                receivedEvents.Add(e.PropertyName);
            };

            evm.Camera = new CameraViewModel(new CameraModel());

            foreach (string s in expectedEvents)
            {
                Assert.IsTrue(receivedEvents.Contains(s));
            }
        }

        [Test]
        public void PictureViewModel_should_notify_on_DisplayName_changed_when_Photographer_LastName_changed()
        {
            List<string> expectedEvents = new List<string>();
            expectedEvents.Add("DisplayName");
            List<string> receivedEvents = new List<string>();
            PictureModel picture = new PictureModel("test");
            picture.EXIF = GetDemoExif();
            picture.IPTC = GetDemoIPTC();

            PictureViewModel pvm = new PictureViewModel(picture);
            pvm.Photographer = new PhotographerViewModel(new PhotorapherModel());

            pvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                receivedEvents.Add(e.PropertyName);
            };

            pvm.Photographer.LastName = "test";

            foreach (string s in expectedEvents)
            {
                Assert.IsTrue(receivedEvents.Contains(s));
            }
        }

        [Test]
        public void PictureViewModel_should_notify_on_DisplayName_changed_when_Photographer_FirstName_changed()
        {
            List<string> expectedEvents = new List<string>();
            expectedEvents.Add("DisplayName");
            List<string> receivedEvents = new List<string>();
            PictureModel picture = new PictureModel("test");
            picture.EXIF = GetDemoExif();
            picture.IPTC = GetDemoIPTC();

            PictureViewModel pvm = new PictureViewModel(picture);
            pvm.Photographer = new PhotographerViewModel(new PhotorapherModel());

            pvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                receivedEvents.Add(e.PropertyName);
            };

            pvm.Photographer.FirstName = "test";

            foreach (string s in expectedEvents)
            {
                Assert.IsTrue(receivedEvents.Contains(s));
            }
        }

        [Test]
        public void SearchViewModel_should_notify_on_command()
        {
            string searchtext = null;
            SearchViewModel svm = new SearchViewModel();
            svm.SearchActivated += delegate (object sender, SearchEventArgs e)
            {
                searchtext = e.Searchtext;
            };

            svm.SearchText = "test";
            svm.OnEnterOrReturn.Execute(null);

            Assert.NotNull(searchtext);
            Assert.AreEqual(searchtext, "test");
        }

        [Test]
        public void BusinessLayer_should_throw_Exception_if_path_is_not_set()
        {
            Assert.That(() => new BusinessLayer(), Throws.Exception);
        }

        [Test]
        public void BusinessLayer_should_throw_Exception_if_path_is_null()
        {
            Assert.That(() => new BusinessLayer(null), Throws.Exception);
        }

        [Test]
        public void BusinessLayer_should_throw_Exception_if_path_is_empty()
        {
            Assert.That(() => new BusinessLayer(""), Throws.Exception);
        }

        [Test]
        public void PictureViewModel_Photographer_should_be_null_if_Photographer_in_PictureModel_is_never_set()
        {
            PictureViewModel pvm = new PictureViewModel(new PictureModel("test"));
            Assert.Null(pvm.Photographer);
        }

        [Test]
        public void PictureViewModel_Camera_should_be_null_if_Camera_in_PictureModel_is_never_set()
        {
            PictureViewModel pvm = new PictureViewModel(new PictureModel("test"));
            Assert.Null(pvm.Camera);
        }

        [Test]
        public void PictureViewModel_Photographer_should_be_not_be_null_if_Photographer_in_PictureModel_is_set()
        {
            PictureViewModel pvm = new PictureViewModel(new PictureModel("test"));
            pvm.Photographer = new PhotographerViewModel(new PhotorapherModel());
            Assert.NotNull(pvm.Photographer);
        }

        [Test]
        public void PictureViewModel_Camera_should_be_not_be_null_if_Camera_in_PictureModel_is_set()
        {
            PictureViewModel pvm = new PictureViewModel(new PictureModel("test"));
            pvm.Camera = new CameraViewModel(new CameraModel());
            Assert.NotNull(pvm.Camera);
        }

        [Test]
        public void DataAccessLayer_should_throw_exception_when_connection_not_available()
        {
            Assert.That(() => new DataAccessLayer("","","",""), Throws.Exception);
        }

        [Test]
        public void BusinessLayer_should_remove_all_references_if_photographer_is_deleted()
        {
            BusinessLayer bl = new BusinessLayer("path");

            InsertPictureModels(3, bl);

            PhotorapherModel photographer = new PhotorapherModel();
            bl.Save(photographer);

            photographer = (PhotorapherModel)bl.GetPhotographers().ElementAt(0);

            List<IPictureModel> pictures = bl.GetPictures().ToList<IPictureModel>();
            ((PictureModel)pictures[0]).Photographer = photographer;

            bl.Save(pictures[0]);

            bl.DeletePhotographer(photographer.ID);

            pictures = bl.GetPictures().ToList<IPictureModel>();

            Assert.Null(((PictureModel)pictures[0]).Photographer);
        }

        [Test]
        public void BusinessLayer_should_remove_all_references_if_camera_is_deleted()
        {
            BusinessLayer bl = new BusinessLayer("path");

            InsertPictureModels(3, bl);

            CameraModel Camera = new CameraModel();
            bl.Save(Camera);

            Camera = (CameraModel)bl.GetCameras().ElementAt(0);

            List<IPictureModel> pictures = bl.GetPictures().ToList<IPictureModel>();
            ((PictureModel)pictures[0]).Camera = Camera;

            bl.Save(pictures[0]);

            bl.DeleteCamera(Camera.ID);

            pictures = bl.GetPictures().ToList<IPictureModel>();

            Assert.Null(((PictureModel)pictures[0]).Camera);
        }


        int InsertPictureModels(int count, BusinessLayer bl)
        {
            for (int i = 0; i < count; i++)
            {
                bl.Save(new PictureModel("test " + i.ToString()));
            }

            return bl.GetPictures().Count() - count;
        }

        List<IPictureViewModel> PictureModelToPictureViewModel(IEnumerable<IPictureModel> list)
        {
            List<IPictureViewModel> pictureViewList = new List<IPictureViewModel>();

            foreach (IPictureModel pm in list)
            {
                pictureViewList.Add(new PictureViewModel(pm));
            }

            return pictureViewList;
        }

        private IEXIFModel GetDemoExif()
        {
            EXIFModel e = new EXIFModel();
            e.Make = "Make";
            e.FNumber = 2;
            e.ExposureTime = 5;
            e.ISOValue = 200;
            e.Flash = true;
            e.ExposureProgram = ExposurePrograms.LandscapeMode;
            return e;
        }

        private IIPTCModel GetDemoIPTC()
        {
            IIPTCModel i = new IPTCModel();
            i.ByLine = "ByLine";
            i.Caption = "Captoin";
            i.CopyrightNotice = "CopyrightNotice";
            i.Headline = "Headline";
            i.Keywords = "Keywords";
            return i;
        }
    }
}

