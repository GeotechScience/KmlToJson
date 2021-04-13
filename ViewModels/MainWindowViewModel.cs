using KmlToJson.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

using System.Text.Json;
using System.Text;
using System.Windows;

namespace KmlToJson.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "GeoMap To GISGeoson";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string inputGeoPath;

        public string InputGeoPath
        {
            get { return inputGeoPath; }
            set { SetProperty(ref inputGeoPath, value); }
        }

        private string outputJsonPath;

        public string OutputJsonPath
        {
            get { return outputJsonPath; }
            set { SetProperty(ref outputJsonPath, value); }
        }


        private string resultLog;

        public string ResultLog
        {
            get { return resultLog; }
            set { SetProperty(ref resultLog, value); }
        }

        public List<GMSite> Sites { get; set; }
        public Dictionary<string, GMInstrumentType> InsTypes { get; set; }

        public ICommand BrowseInputGeoCommand => new DelegateCommand(()=> 
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GeoMap Files|*.kml;*.kms";
            if (openFileDialog.ShowDialog() == true)
            {
                InputGeoPath = openFileDialog.FileName;
            }
                
        });

        public ICommand BrowseOutputJsonCommand => new DelegateCommand(() =>
        {
            var saveFD = new SaveFileDialog();
            saveFD.CheckFileExists = false;
            saveFD.Filter = "Json File|*.json";
            if (saveFD.ShowDialog() == true)
            {
                OutputJsonPath = saveFD.FileName;
            }
        });

        public ICommand OutputCommand => new DelegateCommand<string>((id)=> 
        {
            switch (id)
            {
                default:
                    break;
                case "1":
                    OutputA();
                    break;
                case "2":
                    OutputB();
                    break;
            }
        });

        public MainWindowViewModel()
        {
            InsTypes = new Dictionary<string, GMInstrumentType>();
            Sites = new List<GMSite>();
        }

        void LoadData()
        {
            Sites.Clear();
            InsTypes.Clear();

            using (FileStream fs = new FileStream(InputGeoPath, FileMode.Open))
            {
                var kmlF = KmlFile.Load(fs);
                var kmlR = kmlF.Root as Kml;
                if (kmlR != null)
                {
                    var container = kmlR.Feature as Container;
                    var siteFeatures = container.Features;

                    foreach (var siteFeature in siteFeatures)
                    {
                        var siteFolder = siteFeature as Folder;
                        var gmSite = new GMSite(siteFolder.Name);

                        foreach (var pt in siteFolder.Features)
                        {
                            var place = pt as Placemark;

                            var placeGeo = place.Geometry as SharpKml.Dom.Point;

                            // Console.WriteLine(siteFeature.Name + " - " + place.Name);
                            string placeName = place.Name;
                            if (placeName.StartsWith("*"))
                            {
                                // placeName.Substring(1);
                                gmSite.Latitude = placeGeo.Coordinate.Latitude;
                                gmSite.Longitude = placeGeo.Coordinate.Longitude;
                            }
                            else
                            {
                                int idxLeft = placeName.IndexOf('[');
                                int idxRight = placeName.IndexOf(']');

                                string insTypeName = placeName.Substring(idxLeft + 1, idxRight - idxLeft - 1);
                                string insName = placeName.Substring(idxRight + 1, placeName.Length - idxRight - 1);
                                if (!InsTypes.ContainsKey(insTypeName))
                                {
                                    InsTypes.Add(insTypeName, new GMInstrumentType(insTypeName));
                                }

                                var gmIns = new GMInstrument(insName, placeGeo.Coordinate.Longitude, placeGeo.Coordinate.Latitude,
                                    InsTypes[insTypeName]);
                                gmSite.Instruments.Add(gmIns);
                            }

                            ResultLog += siteFeature.Name + " - " + place.Name + "\r\n";
                        }

                        Sites.Add(gmSite);
                    }
                }
            }
        }

        void OutputA()
        {
            LoadData();

            var sb = new StringBuilder();
            sb.AppendLine("[");

            foreach (var site in Sites)
            {
                sb.AppendLine("{ name: '" + site.Name + "', lat: " + site.Latitude + ", lng: " + site.Longitude + ", geoType: 'station' },");
                foreach (var ins in site.Instruments)
                {
                    sb.AppendLine("{ name: '" + ins.Name + "', lat: " + ins.Latitude + ", lng: " + ins.Longitude + ", geoType: 'instrument', group: '" + site.Name + "' },");
                }
            }

            sb.AppendLine("]");
            string contentToWrite = sb.ToString();
            File.WriteAllText(OutputJsonPath, contentToWrite);
            LogPrependLine("JSON 型 A 輸出成功。");
        }

        void OutputB()
        {
            LoadData();

            var sb = new StringBuilder();
            sb.AppendLine("[");

            foreach (var site in Sites)
            {
                sb.AppendLine($"['{site.Name}', '{site.Name} 站。集電箱。', 'site', {site.Latitude}, {site.Longitude}, 0],");
                foreach (var ins in site.Instruments)
                {
                    sb.AppendLine($"['{ins.Name}', '{ins.Name}。隸屬於「{site.Name}」。', '{ins.Type.Name}', {ins.Latitude}, {ins.Longitude}, 0],");
                }
            }

            sb.AppendLine("]");
            string contentToWrite = sb.ToString();
            File.WriteAllText(OutputJsonPath, contentToWrite);
            LogPrependLine("JSON 型 B 輸出成功。");
        }

        void LogPrependLine(string msg)
        {
            ResultLog = msg + "\r\n" + ResultLog;
        }
    }
}
