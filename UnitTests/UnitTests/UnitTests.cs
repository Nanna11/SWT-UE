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

            foreach(IPictureModel pm in pictureList)
            {
                pictureViewList.Add(new PictureViewModel(pm));
            }

            PictureListViewModel pictureListViewModel = new PictureListViewModel(pictureViewList);
            pictureListViewModel.CurrentIndex = currentIndex;

            Assert.IsTrue(pictureViewList[currentIndex] == pictureListViewModel.CurrentPicture);

            Assert.IsTrue(pictureListViewModel.PrevPictures.Count<IPictureViewModel>() == currentIndex );
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
            int ExistingPictureCount = businessLayer.GetPictures().Count();
            currentIndex += businessLayer.GetPictures().Count();

            for (int i = 0; i < pictureCount; i++)
            {
                businessLayer.Save(new PictureModel("test " + i.ToString()));
            }

            List<IPictureModel> pictureList = businessLayer.GetPictures().ToList<IPictureModel>();
            List<IPictureViewModel> pictureViewList = new List<IPictureViewModel>();

            foreach (IPictureModel pm in pictureList)
            {
                pictureViewList.Add(new PictureViewModel(pm));
            }

            PictureListViewModel pictureListViewModel = new PictureListViewModel(pictureViewList);
            pictureListViewModel.CurrentIndex = currentIndex;

            Assert.IsTrue(pictureViewList[currentIndex] == pictureListViewModel.CurrentPicture);
        }


    }
}
