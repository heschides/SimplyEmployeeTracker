﻿using Microsoft.Win32;
using SimplyEmployeeTracker.DataAccess;
using SimplyEmployeeTracker.Functions;
using SimplyEmployeeTracker.Models;
using SimplyEmployeeTracker.Other;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplyEmployeeTracker.ViewModels
{
    public class AddItemWindowViewModel : ViewModelBase
    {

        //PROPERTIES
        public ObservableCollection<EquipmentAssignmentRecordModel> Assignments { get; set; }
        public ObservableCollection<EquipmentClassModel> EquipmentClasses { get; set; }
        public ObservableCollection<DocumentModel> SelectedFiles { get; set; }
        public ObservableCollection<DocumentModel> Documents { get; set; }

        private string _id;
        public string Id
        {
            get { return _id; }
            set { OnPropertyChanged(ref _id, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { OnPropertyChanged(ref _description, value); }
        }

        private EquipmentClassModel _class;
        public EquipmentClassModel Class
        {
            get { return _class; }
            set { OnPropertyChanged(ref _class, value); }
        }

        private EquipmentStatusModel _status;
        public EquipmentStatusModel Status
        {
            get { return _status; }
            set { OnPropertyChanged(ref _status, value); }
        }

        private string _serialNumber;
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { OnPropertyChanged(ref _serialNumber, value); }
        }

        private string _price;
        public string Price
        {
            get { return _price; }
            set { OnPropertyChanged(ref _price, value); }
        }

        private DateTime _purchaseDate;
        public DateTime PurchaseDate
        {
            get { return _purchaseDate; }
            set { OnPropertyChanged(ref _purchaseDate, value); }
        }
        private string _warrantyMonths;
        public string WarrantyMonths
        {
            get { return _warrantyMonths; }
            set { OnPropertyChanged(ref _warrantyMonths, value); }
        }

        private string _brand;
        public string Brand
        {
            get { return _brand; }
            set { OnPropertyChanged(ref _brand, value); }
        }

        private string _model;
        public string Model
        {
            get { return _model; }
            set { OnPropertyChanged(ref _model, value); }
        }

        private string _po;
        public string PO
        {
            get { return _po; }
            set { OnPropertyChanged(ref _po, value); }
        }

        private bool _cicIsRequired;
        public bool CICIsRequired
        {
            get { return _cicIsRequired; }
            set { OnPropertyChanged(ref _cicIsRequired, value); }
        }

        //COMMANDS
        public RelayCommand<object> GetEquipmentClassesCommand { get; set; }
        public async void GetEquipmentClasses(object e)
        {
            var _equipmentClasses = await GetData.EquipmentClassQueryAsync();
            var Ids = new List<int>();
            foreach (EquipmentClassModel _class in EquipmentClasses) { Ids.Add(_class.Id); }

            foreach (EquipmentClassModel _class in _equipmentClasses)
            {
                if (Ids.Contains(_class.Id))
                { }
                else { EquipmentClasses.Add(_class); }
            }
        }

        public RelayCommand<object> CreateItemCommand { get; set; }
        public void CreateItem(object e)
        {
            var newItem = new EquipmentModel();
            newItem.InventoryId = Id;
            newItem.Class = Class;
            bool result = decimal.TryParse(Price, out decimal price);
            newItem.Price = price;
            var itemStatus = new EquipmentStatusModel
            {
                Id = 2
            };
            newItem.Status = itemStatus;
            newItem.Brand = Brand;
            newItem.PurchaseDate = PurchaseDate;
            newItem.SerialNumber = SerialNumber;
            newItem.Description = Description;
            newItem.Documents = SelectedFiles;
            result = int.TryParse(WarrantyMonths, out int warrantyMonths);
            newItem.WarrantyMonths = warrantyMonths;
            SaveItemDocument();
            SendData.CreateItem(newItem);
            Id = string.Empty;
            Description = string.Empty;
            Class = null;
            Brand = string.Empty;
            Model = string.Empty;
            SerialNumber = string.Empty;
            WarrantyMonths = string.Empty;
            Price = string.Empty;
            PO = string.Empty;
            SelectedFiles.Clear();
        }

        public RelayCommand<object> OpenFileDialogCommand { get; set; }
        public void OpenFileDialogWindow(object e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                var selectedFileSafeNames = openFile.SafeFileNames.ToList();
                var selectedFileNames = openFile.FileNames.ToList();
                foreach (string safeFileName in selectedFileSafeNames)
                {
                    var newDocument = new DocumentModel();
                    newDocument.SafeName = safeFileName;
                    foreach (string fileName in selectedFileNames)
                    {
                        newDocument.Name = fileName;
                    }
                    SelectedFiles.Add(newDocument);
                }
            }
        }

        public void SaveItemDocument()
        {
            foreach (DocumentModel document in SelectedFiles)
            {
                string fileExtension = ".jpg";
                string newName = GenerateRandom.RandomString(8) + fileExtension;
                var newDocument = new DocumentModel();
                newDocument.Name = newName + fileExtension;
                string baseDirectory = @"C:\Users\jwhit\Documents\SimplyEmployeeTrackerSavedDocuments\EquipmentDocuments\";
                string fullPath = Path.Combine(baseDirectory, newName);
                System.IO.File.Copy(document.Name, fullPath);
                Documents.Add(newDocument);
            }
        }

        public AddItemWindowViewModel()
        {
            SelectedFiles = new ObservableCollection<DocumentModel>();
            Documents = new ObservableCollection<DocumentModel>();
            EquipmentClasses = new ObservableCollection<EquipmentClassModel>();
            GetEquipmentClassesCommand = new RelayCommand<object>(GetEquipmentClasses);
            OpenFileDialogCommand = new RelayCommand<object>(OpenFileDialogWindow);
            CreateItemCommand = new RelayCommand<object>(CreateItem);
        }
    }
}
