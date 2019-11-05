using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Edge2XmlToCsv
{
    /// グループデータのルート構造
    [XmlRoot("CaptureGroupGroupData")]
    public class CaptureGroupGroupData
    {
        [XmlElement("SrcImagePath")]
        public string SrcImagePath { get; set; }

        [XmlElement("Coordinate")]
        public string Coordinate { get; set; }

        [XmlElement("Group")]
        public List<Group> GroupList { get; set; }
    }

    /// グループデータのグループ
    public class Group
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Part")]
        public List<Part> PartList { get; set; }
    }

    /// グループデータのパーツ
    public class Part
    {
        [XmlElement("Use")]
        public string Use { get; set; }

        [XmlElement("Lock")]
        public string Lock { get; set; }

        [XmlElement("Group")]
        public string Group { get; set; }

        [XmlElement("DestPos")]
        public string DestPos { get; set; }

        [XmlElement("Transparent")]
        public string Transparent { get; set; }

        [XmlElement("Invert")]
        public string Invert { get; set; }

        [XmlElement("Rotate")]
        public string Rotate { get; set; }

        [XmlElement("BondMode")]
        public string BondMode { get; set; }

        [XmlElement("Alpha")]
        public string Alpha { get; set; }

        [XmlElement("SrcPageIndex")]
        public string SrcPageIndex { get; set; }

        [XmlElement("SrcPaletteIndex")]
        public string SrcPaletteIndex { get; set; }

        [XmlElement("SrcRect")]
        public string SrcRect { get; set; }

        public string ToCsv()
        {
            return string.Join(",", new string[] {
                    this.Use, this.Lock, this.Group, this.DestPos, this.Transparent, this.Invert,
                    this.Rotate, this.BondMode, this.Alpha, this.SrcPageIndex, this.SrcPaletteIndex, this.SrcRect});
        }
    }

    /// シーケンスデータのルート構造
    [XmlRoot("CaptureGroupSequenceData")]
    public class CaptureGroupSequenceData
    {
        [XmlElement("DelayUnit")]
        public string DelayUnit { get; set; }

        [XmlElement("Frame")]
        public List<Frame> FrameList { get; set; }
    }

    /// シーケンスデータのフレーム
    public class Frame
    {
        [XmlElement("Use")]
        public string Use { get; set; }

        [XmlElement("SrcGroup")]
        public string SrcGroup { get; set; }

        [XmlElement("DestPos")]
        public string DestPos { get; set; }

        [XmlElement("Delay")]
        public string Delay { get; set; }

        [XmlElement("AfterMode")]
        public string AfterMode { get; set; }

        public string ToCsv()
        {
            return string.Join(",", new string[] {this.Use, this.SrcGroup, this.DestPos, this.Delay, this.AfterMode});
        }
    }

    /// グループデータの解析
    public class CggParser
    {
        public CggParser(XmlDocument doc)
        {
            var serializer = new XmlSerializer(typeof(CaptureGroupGroupData));
            _model = (CaptureGroupGroupData)serializer.Deserialize(new XmlNodeReader(doc));
        }

        public void Output(CsvBuilder builder)
        {
            System.Diagnostics.Debug.WriteLine(_model.SrcImagePath);
            System.Diagnostics.Debug.WriteLine(_model.Coordinate);

            foreach (var group in _model.GroupList)
            {
                System.Diagnostics.Debug.WriteLine(group.Name);

                string line = group.Name;
                line += "," + group.PartList.Count;

                foreach (var part in group.PartList)
                {
                    System.Diagnostics.Debug.WriteLine("Use=" + part.Use);
                    System.Diagnostics.Debug.WriteLine("Lock=" + part.Lock);
                    System.Diagnostics.Debug.WriteLine("Group=" + part.Group);
                    System.Diagnostics.Debug.WriteLine("DestPos=" + part.DestPos);
                    System.Diagnostics.Debug.WriteLine("Transparent=" + part.Transparent);
                    System.Diagnostics.Debug.WriteLine("Invert=" + part.Invert);
                    System.Diagnostics.Debug.WriteLine("Rotate=" + part.Rotate);
                    System.Diagnostics.Debug.WriteLine("BondMode=" + part.BondMode);
                    System.Diagnostics.Debug.WriteLine("Alpha=" + part.Alpha);
                    System.Diagnostics.Debug.WriteLine("SrcPageIndex=" + part.SrcPageIndex);
                    System.Diagnostics.Debug.WriteLine("SrcPaletteIndex=" + part.SrcPaletteIndex);
                    System.Diagnostics.Debug.WriteLine("SrcRect=" + part.SrcRect);

                    line += "," + part.ToCsv();
                }

                builder.WriteLine(line);
            }
        }

        CaptureGroupGroupData _model = null;
    }

    /// シーケンスデータの解析
    public class CgsParser
    {
        public CgsParser(XmlDocument doc)
        {
            var serializer = new XmlSerializer(typeof(CaptureGroupSequenceData));
            _model = (CaptureGroupSequenceData)serializer.Deserialize(new XmlNodeReader(doc));
        }

        public void Output(CsvBuilder builder)
        {
            System.Diagnostics.Debug.WriteLine(_model.DelayUnit);

            foreach (var frame in _model.FrameList)
            {
                System.Diagnostics.Debug.WriteLine("Use=" + frame.Use);
                System.Diagnostics.Debug.WriteLine("SrcGroup=" + frame.SrcGroup);
                System.Diagnostics.Debug.WriteLine("DestPos=" + frame.DestPos);
                System.Diagnostics.Debug.WriteLine("Delay=" + frame.Delay);
                System.Diagnostics.Debug.WriteLine("AfterMode=" + frame.AfterMode);

                builder.WriteLine(frame.ToCsv());
            }
        }

        CaptureGroupSequenceData _model = null;
    }

    /// CSVの出力
    public class CsvBuilder : IDisposable
    {
        public CsvBuilder(string csvFileName)
        {
            _writer = new StreamWriter(csvFileName, false);
        }

        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("Dispose");

            _writer.Dispose();
        }

        public void WriteLine(string line)
        {
            _writer.WriteLine(line);
        }

        StreamWriter _writer = null;
    }

    class Program
    {
        static void Main(string[] args)
        {
            // 引数チェック
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Edge2XmlToCsv.exe <xmlFileName> <csvFileName>");
                Environment.Exit(1);
                return;
            }

            // XMLファイルが存在するか確認する
            var xmlFileName = args[0];
            if (!File.Exists(xmlFileName))
            {
                Console.WriteLine("ファイルが存在しません: " + xmlFileName);
                Environment.Exit(1);
                return;
            }

            // XMLファイルを読み込む
            var inputStream = new FileStream(xmlFileName, FileMode.Open);

            // CSVビルダーを生成する
            var csvFileName = args[1];
            using (var csvBuilder = new CsvBuilder(csvFileName))
            {
                // ルートノードを取得する
                var doc = new XmlDocument();
                doc.Load(inputStream);
                var rootNode = doc.DocumentElement;

                // ルートノードからデータ内容を判断して解析する
                if (rootNode.Name == "CaptureGroupGroupData")
                {
                    var cggParser = new CggParser(doc);
                    cggParser.Output(csvBuilder);
                }
                else if (rootNode.Name == "CaptureGroupSequenceData")
                {
                    var cgsParser = new CgsParser(doc);
                    cgsParser.Output(csvBuilder);
                }
                else
                {
                    Console.WriteLine("未対応");
                    Environment.Exit(1);
                    return;
                }
            }
        }
    }
}
