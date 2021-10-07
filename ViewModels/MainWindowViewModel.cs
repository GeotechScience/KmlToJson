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
            openFileDialog.Filter = "GeoMap Files|*.kml;*.kmz";
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
                    OutputJson_A();
                    break;
                case "2":
                    OutputJson_B();
                    break;
                case "CeciRetainingWallLine":
                    OutputJson_CeciRetainingWallLine();
                    break;
                case "ExtractTextPlace":
                    ExtractFoldersContainsName("文字");
                    break;
                case "ExtractNameCoordToArray":
                    ExtractNameCoordToArray();
                    break;
                case "FPPT_ExtractNameCoordToArray":
                    FPPT_ExtractNameCoordToArray();
                    break;
            }
        });

        private void FPPT_ExtractNameCoordToArray()
        {
            using (FileStream fs = new FileStream(InputGeoPath, FileMode.Open))
            {


                var kmlF = KmlFile.Load(fs);
                var kmlR = (kmlF.Root as SharpKml.Dom.Kml).Feature as Folder;

                var listPointsWithName = new List<PointWithNameSimple>();

                if (kmlR != null)
                {
                    // var container = kmlR.Feature as Container;
                    var features = kmlR.Features;
                    
                    foreach (var feature in features)
                    {
                        var placemark = feature as Placemark;
                        if (placemark != null)
                        {
                            var ptWithName = new PointWithNameSimple();
                            var pm = placemark;
                            ptWithName.Name = pm.Name;

                            var geo = pm.Geometry as SharpKml.Dom.Point;
                            ptWithName.Longi = (float)geo.Coordinate.Longitude;
                            ptWithName.Lati = (float)geo.Coordinate.Latitude;
                            listPointsWithName.Add(ptWithName);
                            // ofmc.Coords = geo.;
                            // ofmc.Coords = folder.Features.First().

                            // listFolders.Add(new OneFolderMultiCoords { Coords })
                        }
                    }
                    //var fakeKML = KmlFile.Create(fakeKMLDoc, true);


                    //using (var memStream = new MemoryStream())
                    //{
                    //    fakeKML.Save(memStream);

                    //    //using (var sR = new StreamReader(memStream))
                    //    //{
                    //    //    string text = sR.ReadToEnd();
                    //    //    MessageBox.Show(text);
                    //    //}

                    //    string text = Encoding.UTF8.GetString(memStream.ToArray());
                    //    // Console.WriteLine(text);
                    //    Clipboard.SetText(text);
                    //    MessageBox.Show("Copied to clipboard!");
                    //}



                    string json = System.Text.Json.JsonSerializer.Serialize(listPointsWithName, new JsonSerializerOptions { WriteIndented = true });
                    // Clipboard.SetText(json);
                    File.WriteAllText(OutputJsonPath, json);
                    LogPrependLine("JSON Name 輸出成功。");
                }
            }
        }

        private void ExtractNameCoordToArray()
        {
            using (FileStream fs = new FileStream(InputGeoPath, FileMode.Open))
            {


                var kmlF = KmlFile.Load(fs);
                var kmlR = kmlF.Root as Document;

                var listPointsWithName = new List<PointWithNameSimple>();

                if (kmlR != null)
                {
                    // var container = kmlR.Feature as Container;
                    var features = kmlR.Features;

                    foreach (var feature in features)
                    {
                        var folder = feature as Folder;
                        if (folder != null)
                        {
                            var ptWithName = new PointWithNameSimple();
                            var pm = folder.Features.First() as Placemark;
                            ptWithName.Name = pm.Name;

                            var geo = pm.Geometry as SharpKml.Dom.Point;
                            ptWithName.Longi = (float) geo.Coordinate.Longitude;
                            ptWithName.Lati = (float) geo.Coordinate.Latitude;
                            listPointsWithName.Add(ptWithName);
                            // ofmc.Coords = geo.;
                            // ofmc.Coords = folder.Features.First().

                            // listFolders.Add(new OneFolderMultiCoords { Coords })
                        }
                    }
                    //var fakeKML = KmlFile.Create(fakeKMLDoc, true);


                    //using (var memStream = new MemoryStream())
                    //{
                    //    fakeKML.Save(memStream);

                    //    //using (var sR = new StreamReader(memStream))
                    //    //{
                    //    //    string text = sR.ReadToEnd();
                    //    //    MessageBox.Show(text);
                    //    //}

                    //    string text = Encoding.UTF8.GetString(memStream.ToArray());
                    //    // Console.WriteLine(text);
                    //    Clipboard.SetText(text);
                    //    MessageBox.Show("Copied to clipboard!");
                    //}

                    

                    string json = System.Text.Json.JsonSerializer.Serialize(listPointsWithName, new JsonSerializerOptions { WriteIndented = true });
                    // Clipboard.SetText(json);
                    File.WriteAllText(OutputJsonPath, json);
                    LogPrependLine("JSON Name 輸出成功。");
                }
            }
        }

        public List<CeciRetainingWallLine> CeciRetainingWallLine_items { get; private set; }

        public MainWindowViewModel()
        {
            InsTypes = new Dictionary<string, GMInstrumentType>();
            Sites = new List<GMSite>();
        }

        void LoadSiteAndInstrumentsData()
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

        void OutputJson_A()
        {
            LoadSiteAndInstrumentsData();

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

        void OutputJson_B()
        {
            LoadSiteAndInstrumentsData();

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

        void OutputJson_CeciRetainingWallLine()
        {
            LoadData_CeciRetainingWallLine();

            var sb = new StringBuilder();
            sb.Append("let ceciRetainingWallDataItems = ");

            string inner = System.Text.Json.JsonSerializer.Serialize(CeciRetainingWallLine_items, new JsonSerializerOptions { WriteIndented = true });
            sb.AppendLine(inner);

            //foreach (var site in Sites)
            //{
            //    sb.AppendLine($"['{site.Name}', '{site.Name} 站。集電箱。', 'site', {site.Latitude}, {site.Longitude}, 0],");
            //    foreach (var ins in site.Instruments)
            //    {
            //        sb.AppendLine($"['{ins.Name}', '{ins.Name}。隸屬於「{site.Name}」。', '{ins.Type.Name}', {ins.Latitude}, {ins.Longitude}, 0],");
            //    }
            //}

            string contentToWrite = sb.ToString();
            File.WriteAllText(OutputJsonPath, contentToWrite);

            LogPrependLine("JSON 型 CECI擋土牆 輸出成功。");
        }

        void LoadData_CeciRetainingWallLine()
        {
            using (FileStream fs = new FileStream(InputGeoPath, FileMode.Open))
            {
                var kmlF = KmlFile.Load(fs);
                var kmlR = kmlF.Root as Kml;
                if (kmlR != null)
                {
                    var container = kmlR.Feature as Container;
                    var siteFeatures = container.Features;

                    CeciRetainingWallLine_items = new List<CeciRetainingWallLine>();

                    foreach (var siteFeature in siteFeatures)
                    {
                        var siteFolder = siteFeature as Folder;
                        foreach (var fPm in siteFolder.Features)
                        {
                            var item = new CeciRetainingWallLine();

                            var placemark = fPm as Placemark;

                            if (placemark.ExtendedData != null)
                            {
                                var schemalD = placemark.ExtendedData.SchemaData.FirstOrDefault();
                                foreach (var simpleData in schemalD.SimpleData)
                                {
                                    switch (simpleData.Name)
                                    {
                                        case "MSISID":
                                            item.Url = simpleData.Text;
                                            break;

                                        case "LEVEL":
                                            item.Level = simpleData.Text;
                                            break;

                                        case "Url":
                                            item.Url = simpleData.Text;
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }

                            var mg = placemark.Geometry as MultipleGeometry;
                            var line = mg.Geometry.FirstOrDefault() as LineString;

                            foreach (var dotOfLine in line.Coordinates)
                            {
                                item.LineDots.Add(dotOfLine);
                            }

                            CeciRetainingWallLine_items.Add(item);
                        }
                    }

                }
            }
        }

        void ExtractFoldersContainsName(string name)
        {
            using (FileStream fs = new FileStream(InputGeoPath, FileMode.Open))
            {
                

                var kmlF = KmlFile.Load(fs);
                var kmlR = kmlF.Root as Kml;

                var fakeKMLDoc = new Document();
                //var rootFolder = new Folder();
                //fakeKMLDoc.AddFeature(rootFolder);

                if (kmlR != null)
                {
                    var container = kmlR.Feature as Container;
                    var features = container.Features;

                    foreach (var feature in features)
                    {
                        var folder = feature as Folder;
                        if (folder.Features != null && folder.Features.Count > 0)
                        {
                            foreach (var subFeature in folder.Features)
                            {
                                
                                var subFolder = subFeature as Folder;
                                if (subFolder != null)
                                {
                                    if (subFolder.Name.Contains(name)) 
                                    {
                                        fakeKMLDoc.AddFeature(subFolder.Clone());
                                    }
                                }
                            }
                        }

                    }
                    var fakeKML = KmlFile.Create(fakeKMLDoc, true);


                    using (var memStream = new MemoryStream())
                    {
                        fakeKML.Save(memStream);

                        //using (var sR = new StreamReader(memStream))
                        //{
                        //    string text = sR.ReadToEnd();
                        //    MessageBox.Show(text);
                        //}
                        
                        string text = Encoding.UTF8.GetString(memStream.ToArray());
                        // Console.WriteLine(text);
                        Clipboard.SetText(text);
                        MessageBox.Show("Copied to clipboard!");
                    }

                }
            }
        }

        void LogPrependLine(string msg)
        {
            ResultLog = msg + "\r\n" + ResultLog;
        }
    }
}
