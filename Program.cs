using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDollarGestureRecognizer;
using System.Windows.Forms;


namespace pdollar
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Point> xyPoints = new List<Point>();
            List<Gesture> gestures = new List<Gesture>();
            if (args.Length == 0)
            {
                Console.WriteLine("Help Menu :");
                Console.WriteLine("pdollar –t <gesturefile>");
                Console.WriteLine("Adds the gesture file to the list of gesture templates");
                Console.WriteLine();
                Console.WriteLine("pdollar ‐r");
                Console.WriteLine("Clears the templates");
                Console.WriteLine();
                Console.WriteLine("pdollar < eventstream >");
                Console.WriteLine("Prints the name of gestures as they are recognized from the event stream");
                Console.WriteLine();
            }
            else
            {
                if(args[0] == "-t")
                {
                    string path = args[1];
                    Console.WriteLine(path);
                    if (File.Exists(path))
                    {
                        using(StreamReader sr = new StreamReader(path))
                        {
                            String str = null;
                            int stroke = 0;
                            String strGesture = sr.ReadLine();
                            while((str = sr.ReadLine()) != null)
                            {
                                if(str == "BEGIN")
                                {
                                    stroke++;
                                }
                                else if(str == "END")
                                {
                                    continue;
                                }
                                else
                                {
                                    Point pt = null;
                                    String[] strArray = str.Split(',');
                                    float xCoordinate = float.Parse(strArray[0], CultureInfo.InvariantCulture.NumberFormat);
                                    float yCoordinate = float.Parse(strArray[1], CultureInfo.InvariantCulture.NumberFormat);
                                    pt = new Point(xCoordinate, yCoordinate, stroke);
                                    xyPoints.Add(pt);
                                }
                            }
                            if (!Directory.Exists(Application.StartupPath + "\\GestureSet\\NewGestures"))
                            { 
                                Directory.CreateDirectory(Application.StartupPath + "\\GestureSet\\NewGestures");
                            }
                            GestureIO.WriteGesture(xyPoints.ToArray(),strGesture,
                                String.Format("{0}\\GestureSet\\NewGestures\\{1}.xml", Application.StartupPath, strGesture)
                            );

                            Console.WriteLine("New gesture registered!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("File not found!!!!!");
                    }
                }
                else if(args[0] == "-r")
                {
                    DirectoryInfo di = new DirectoryInfo(String.Format("{0}\\GestureSet\\NewGestures\\", Application.StartupPath));

                    foreach (FileInfo file in di.GetFiles())
                    {
                        if(file.Extension == ".xml")
                        file.Delete();
                    }
                    Console.WriteLine("Gesture templates deleted");
                }
                else
                {
                    string path = args[0];
                    if (File.Exists(path))
                    {
                        using (StreamReader sr = new StreamReader(path))
                        {
                            String str = null;
                            int stroke = 0;
                            while ((str = sr.ReadLine()) != null)
                            {
                                if (str == "MOUSEDOWN")
                                {
                                    stroke++;
                                }
                                else if (str == "MOUSEUP")
                                {
                                    continue;
                                }
                                else if(str == "RECOGNIZE")
                                {
                                    if (!Directory.Exists(Application.StartupPath + "\\GestureSet\\NewGestures"))
                                    {
                                        Console.WriteLine("Gestures need to be registered.");
                                        Directory.CreateDirectory(Application.StartupPath + "\\GestureSet\\NewGestures");
                                    }
                                    
                                    string[] gestureFolders = Directory.GetDirectories(Application.StartupPath + "\\GestureSet");
                                    foreach (string folder in gestureFolders)
                                    {
                                        string[] gestureFiles = Directory.GetFiles(folder, "*.xml");
                                        foreach (string file in gestureFiles)
                                            gestures.Add(GestureIO.ReadGesture(file));
                                    }
                                    Gesture[] trainingSet = gestures.ToArray();

                                    Gesture candidate = new Gesture(xyPoints.ToArray());
                                    string gestureClass = PointCloudRecognizer.Classify(candidate, trainingSet);
                                    Console.WriteLine("Recognized as: " + gestureClass);
                                }
                                else
                                {
                                    Point pt = null;
                                    String[] strArray = str.Split(',');
                                    float xCoordinate = float.Parse(strArray[0], CultureInfo.InvariantCulture.NumberFormat);
                                    float yCoordinate = float.Parse(strArray[1], CultureInfo.InvariantCulture.NumberFormat);
                                    pt = new Point(xCoordinate, yCoordinate, stroke);
                                    xyPoints.Add(pt);
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("File not found!!!!!");
                    }
                }
            }
        }
    }
}
