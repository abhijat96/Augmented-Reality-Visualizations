using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


class Restaurant
{
    public Dictionary<string, string> features;

    public Restaurant()
    {
        features = new Dictionary<string, string>();
    }
}

class RestaurantObject
{
    public GameObject restaurantGameObject;
    public Restaurant restaurantDetails;
}

class CuisineCount
{
    public string Name;
    public int Count;
}

public class ARCameraScript : MonoBehaviour
{

    string[] Features;
    private string SelectedArea;
    Dictionary<string, Dictionary<string, RestaurantObject>> BoroWiseDict;
    Dictionary<string, RestaurantObject> curBoroRestaurantDict;
    List<Color> barColors;
    public GameObject ARText;
    public GameObject ARAreaText;
    public GameObject PlaneFinder;
    public GameObject GroundPlaneStage;

    public GameObject BrooklynPlane;
    public GameObject BronxPlane;
    public GameObject StatenIslandPlane;
    public GameObject QueensPlane;
    public GameObject ManhattanPlane;
    public GameObject NYCPlane;
    public GameObject BrooklynCollider;
    public GameObject BronxCollider;
    public GameObject QueensCollider;
    public GameObject StatenIslandCollider;
    public GameObject ManhattanCollider;



    public GameObject Restaurant3DGameObject;

    //TESTING
    private GameObject destroyBar;
    private bool IsTestObjectDestroyed = false;
    private List<RestaurantObject> allVisibleCubes;


    //IMAGE TARGET
    public GameObject ImageTarget;
    //BAR GRAPH
    private GameObject curBarGraph;
    public GameObject BarGraph;
    public GameObject Bar;
    public GameObject origin;
    public GameObject curBar;
    public GameObject YValueLabel;
    public List<GameObject> BarGraphHeights1;
    public List<GameObject> BarGraphHeights2;
    public GameObject LineChartTrend1;
    public GameObject LineChartTrend2;
    private LineRenderer Trend1LineRenderer;
    private LineRenderer Trend2LineRenderer;

    //LINE CHART
    private GameObject curLineChart;
    public GameObject LineChart;

    ////UI
    public GameObject FinalizePlaneButton;
    public GameObject SelectAreaButton;
    public GameObject ButtonZoomOut;
    public Joystick LeftJoystick;
    public Joystick RightJoystick;
    public Slider slider;
    public GameObject ButtonBrooklyn;
    public GameObject ButtonManhattan;
    public GameObject ButtonBronx;
    public GameObject ButtonQueens;
    public GameObject ButtonStatenIsland;


    public GameObject currentObject;
    private GameObject currentBoro;

    //HALO EFFECT
    private static float HaloChange = 0.005f;

    //private int TouchCount = 0;

    //MODES
    private bool PlaneEstablished = false;
    private bool ModePlaneSelection = true;
    private bool ModeSelection = false;
    private bool ModeBuild = false;
    private bool ModePlay = false;
    

    private bool InTransitionZoomIn = false;
    private bool InTransitionZoomOut = false;
    private bool ZoomedIn = false;
    private Vector3 transitionTargetPosition;
    public GameObject ImagePlane;
    private GameObject BiggestPlane;
    private bool HasZoomed = false;
    private string selectedBoro;
    private int selectedThreshold;

    //DEBUGGING
    private static bool ExceptionOccured = false;
    private static string LastExceptionString = "Default";

    void FinalizePlane()
    {
        PlaneEstablished = true;
        PlaneFinder.SetActive(false);
        ModePlaneSelection = false;
        ModeSelection = true;
        FinalizePlaneButton.SetActive(false);
        SelectAreaButton.SetActive(true);
    }

    void MoveToSelectedArea()
    {
        if ((currentObject.transform.name.StartsWith("Brooklyn"))
            ||
            (currentObject.transform.name.StartsWith("StatenIsland"))
            ||
            (currentObject.transform.name.StartsWith("Bronx"))
            ||
            (currentObject.transform.name.StartsWith("Queens"))
            ||
            (currentObject.transform.name.StartsWith("Manhattan"))
            )
        {
            InTransitionZoomIn = true;
            ModeSelection = false;
            transitionTargetPosition = new Vector3(transform.position.x,
                                                   transform.position.y - 7f,
                                                   transform.position.z);
        }
        
    }

    void TransitionMapTowardsCamera()
    {
        if (!HasZoomed)
        {
            float ScaleSpeed = 0.2f;
            if (BiggestPlane.transform.localScale.x < 2f)
            {
                BiggestPlane.transform.localScale = new Vector3(BiggestPlane.transform.localScale.x + ScaleSpeed, BiggestPlane.transform.localScale.y, ImagePlane.transform.localScale.z + ScaleSpeed);
            }
            else
            {
                HasZoomed = true;
                currentObject.transform.parent = null;
                ImagePlane.transform.parent = currentObject.transform;
                BiggestPlane = currentObject;
            }
        }
        else
        {            
            float speed = 4f;
            float step = speed * Time.deltaTime;

            //transform.position = transitionTargetPosition;
            if ((Math.Abs(BiggestPlane.transform.position.x - transitionTargetPosition.x) > 1f) ||
                    (Math.Abs(BiggestPlane.transform.position.y - transitionTargetPosition.y) > 1f) ||
                    (Math.Abs(BiggestPlane.transform.position.z - transitionTargetPosition.z) > 1f))
            {
                currentObject.transform.position = Vector3.MoveTowards(BiggestPlane.transform.position
                                                                        , transitionTargetPosition, step);

            }
            else
            {
                InTransitionZoomIn = false;
                ModeBuild = true;
                ZoomedIn = true;

                ImagePlane.transform.parent = GroundPlaneStage.transform;
                currentObject.transform.parent = ImagePlane.transform;
                BiggestPlane = ImagePlane;
                ButtonZoomOut.SetActive(true);

            }
        }
        
    }

    void SwitchToZoomOutMode()
    {
        InTransitionZoomOut = true;
        BiggestPlane = ImagePlane;
        currentObject.transform.parent = ImagePlane.transform;
        transitionTargetPosition = new Vector3(transform.position.x,
                                               transform.position.y - 2f,
                                               transform.position.z);
    }

    void ZoomOut()
    {

        if (BiggestPlane.transform.localScale.x > 0.5f)
        {
            BiggestPlane.transform.localScale = new Vector3(
                                    BiggestPlane.transform.localScale.x - 0.05f,
                                    BiggestPlane.transform.localScale.y,
                                    BiggestPlane.transform.localScale.z - 0.05f);
        }
        else
        {
            if ((Math.Abs(BiggestPlane.transform.position.x - transitionTargetPosition.x) > 1f) ||
                    (Math.Abs(BiggestPlane.transform.position.y - transitionTargetPosition.y) > 1f) ||
                    (Math.Abs(BiggestPlane.transform.position.z - transitionTargetPosition.z) > 1f))
            {
                float speed = 4f;
                float step = speed * Time.deltaTime;
                BiggestPlane.transform.position = Vector3.MoveTowards(BiggestPlane.transform.position,
                                                                    transitionTargetPosition,
                                                                    step);
            }
            else
            {
                InTransitionZoomOut = false;
                ModeSelection = true;
                ImagePlane.transform.parent = GroundPlaneStage.transform;
                HasZoomed = false;
                ZoomedIn = false;
            }
        }
    }

    void SwitchToBuildMode()
    {
        ModeSelection = false;
        ModeBuild = true;
        ModePlay = false;
        ARText.GetComponent<TextMesh>().text = "";
    }

    void SwitchToPlayMode()
    {
        ModeSelection = false;
        ModeBuild = false;
        ModePlay = true;
    }

    void ChangeThreshold(float sliderValue)
    {
        try
        {
            GameObject SliderLabel = slider.transform.Find("Label").gameObject;
            int highestPossibleScore = 100;
            selectedThreshold = (int)(((100 - sliderValue) / 100) * highestPossibleScore);
            SliderLabel.GetComponent<Text>().text = "Threshold Score: " + selectedThreshold.ToString();
            CreateAndPlaceCubesOnMap();
        }
        catch(Exception ex)
        {
            ExceptionOccured = true;
            UnityEngine.Debug.Log("EXCEPTION!!!!!");
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LastExceptionString = "THRESOLD change";
        }
    }

    void ApplyBrooklynFilter()
    {
        selectedBoro = "BROOKLYN";
        currentObject = BrooklynCollider;
        currentBoro = BrooklynPlane;
        CreateAndPlaceCubesOnMap();
        
    }

    void ApplyStatenIslandFilter()
    {
        selectedBoro = "STATEN ISLAND";
        currentObject = StatenIslandCollider;
        currentBoro = StatenIslandPlane;
        CreateAndPlaceCubesOnMap();
        
    }

    void ApplyBronxFilter()
    {
        selectedBoro = "BRONX";
        currentObject = BronxCollider;
        currentBoro = BronxPlane;
        CreateAndPlaceCubesOnMap();
        
    }
    void ApplyQueensFilter()
    {
        selectedBoro = "QUEENS";
        currentObject = QueensCollider;
        currentBoro = QueensPlane;
        CreateAndPlaceCubesOnMap();
    }
    void ApplyManhattanFilter()
    {
        selectedBoro = "MANHATTAN";
        currentObject = ManhattanCollider;
        currentBoro = ManhattanPlane;
        CreateAndPlaceCubesOnMap();
    }

    void CreateBarGraph(string[] XValues, float[] YValues, float YValueMax, string XAxisLabel, string YAxisLabel)
    {
        try
        {
            Destroy(curBarGraph);

            curBarGraph = Instantiate(BarGraph);
            curBarGraph.SetActive(true);
            curBarGraph.transform.parent = ImageTarget.transform;
            curBarGraph.transform.position = BarGraph.transform.position;
            curBarGraph.transform.Find("XAxisText").gameObject.GetComponent<TextMesh>().text = XAxisLabel;
            curBarGraph.transform.Find("YAxisText").gameObject.GetComponent<TextMesh>().text = YAxisLabel;
            float lastYLabel = -1f;
            BarGraph.SetActive(false);
            //curBarGraph.transform.rotation = BarGraph.transform.rotation;


            int barCount = YValues.Length;
            int YDivisionCount = 10;

            if (YValueMax == -1f)
            {
                foreach (float curValue in YValues)
                {
                    if (curValue > YValueMax)
                        YValueMax = curValue;
                }
            }
            YValueMax = YValueMax + (YValueMax * 0.1f);

            origin = curBarGraph.transform.Find("Origin").gameObject;
            GameObject XEnd = curBarGraph.transform.Find("XEnd").gameObject;
            GameObject YEnd = curBarGraph.transform.Find("YEnd").gameObject;

            float gapBetweenBars = (XEnd.transform.position.x - origin.transform.position.x) / barCount;
            float YLabelOffset = (XEnd.transform.position.x - origin.transform.position.x) / 15;
            float XOffset = gapBetweenBars / 1000;
            float YOffset = gapBetweenBars / 10;
            float gapBetweenYValueLabels = (YEnd.transform.position.y - origin.transform.position.y) / YDivisionCount;

            for (int i = 0; i < YDivisionCount; i++)
            {
                GameObject curYLabel = Instantiate(YValueLabel);
                curYLabel.transform.parent = curBarGraph.transform;
                curYLabel.transform.position = new Vector3(origin.transform.position.x - YLabelOffset * 2,
                                                            origin.transform.position.y + gapBetweenYValueLabels + (gapBetweenYValueLabels * i),
                                                            origin.transform.position.z
                                                            );
                curYLabel.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                curYLabel.transform.GetComponent<TextMesh>().text = ((int)((YValueMax / YDivisionCount) * i)).ToString();
            }

            for (int i = 0; i < barCount; i++)
            {
                curBar = Instantiate(Bar);
                curBar.SetActive(true);
                curBar.transform.parent = curBarGraph.transform;
                curBar.transform.Find("CubeBase").Find("Cube").GetComponent<Renderer>().material.color = barColors[i];
                curBar.transform.localScale = new Vector3(curBar.transform.localScale.x / 100,
                                                           curBar.transform.localScale.y / 100,
                                                            curBar.transform.localScale.z / 100);
                curBar.transform.position = new Vector3(origin.transform.position.x + XOffset + (gapBetweenBars * (i + 1)), origin.transform.position.y + YOffset, origin.transform.position.z);
                curBar.transform.Find("Label").GetComponent<TextMesh>().text = XValues[i];

                float targetHeight = (YValues[i] / YValueMax) * (YEnd.transform.localPosition.y - 11.58f);

                GameObject curCubeBase = curBar.transform.Find("CubeBase").gameObject;
                GameObject curHeight = curCubeBase.transform.Find("BarHeightPosition").gameObject;
                curHeight.transform.parent = curBarGraph.transform;

                while (curHeight.transform.localPosition.y - 11.58f < targetHeight)
                {

                    curHeight.transform.parent = curCubeBase.transform;
                    curCubeBase.transform.localScale = new Vector3(curCubeBase.transform.localScale.x,
                                                                    curCubeBase.transform.localScale.y + 0.3f,
                                                                    curCubeBase.transform.localScale.z);
                    curCubeBase.transform.localPosition = new Vector3(curCubeBase.transform.position.x,
                                                                       (curCubeBase.transform.localScale.y / 2) - 0.5f,
                                                                        curCubeBase.transform.position.z);

                    curHeight.transform.parent = curBarGraph.transform;
                }
                curHeight.transform.parent = curCubeBase.transform;
            }
        }
        catch(Exception ex)
        {
            ExceptionOccured = true;
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LastExceptionString = "Ex! Bar Graph!";
        }
    }

    void CreateLineChart(string[] XValues, float[] YValues1, float[] YValues2, string XAxisLabel, string YAxisLabel)
    {

        Destroy(curLineChart);
        curLineChart = Instantiate(LineChart);
        curLineChart.transform.parent = ImageTarget.transform;
        curLineChart.transform.position = LineChart.transform.position;
        curLineChart.transform.Find("XAxisText").gameObject.GetComponent<TextMesh>().text = XAxisLabel;
        curLineChart.transform.Find("YAxisText").gameObject.GetComponent<TextMesh>().text = YAxisLabel;
        LineChart.SetActive(false);
        curLineChart.SetActive(true);

        BarGraphHeights1 = new List<GameObject>();
        BarGraphHeights2 = new List<GameObject>();

        int barCount = XValues.Length;
        int YDivisionCount = 10;

        int YValueMax = -1;
        foreach(float curVal in YValues1)
        {
            if (curVal > YValueMax)
                YValueMax = (int)curVal;
        }

        foreach (float curVal in YValues2)
        {
            if (curVal > YValueMax)
                YValueMax = (int)curVal;
        }

        origin = curLineChart.transform.Find("Origin").gameObject;
        GameObject XEnd = curLineChart.transform.Find("XEnd").gameObject;
        GameObject YEnd = curLineChart.transform.Find("YEnd").gameObject;

        float gapBetweenBars = (XEnd.transform.position.x - origin.transform.position.x) / barCount;
        float XOffset = gapBetweenBars / 1000;
        float YOffset = gapBetweenBars / 10;

        #region Labelling Y Axis
        float gapBetweenYValueLabels = (YEnd.transform.position.y - origin.transform.position.y) / YDivisionCount;
        for (int i = 0; i < YDivisionCount; i++)
        {
            GameObject curYLabel = Instantiate(YValueLabel);
            curYLabel.transform.parent = curLineChart.transform;
            curYLabel.transform.position = new Vector3(origin.transform.position.x - gapBetweenBars*2,
                                                        origin.transform.position.y + gapBetweenYValueLabels + (gapBetweenYValueLabels * i),
                                                        origin.transform.position.z
                                                        );
            curYLabel.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            curYLabel.transform.GetComponent<TextMesh>().text = ((YValueMax / YDivisionCount) * i).ToString();
        }
        #endregion

        #region Trend 1
        for (int i = 0; i < barCount; i++)
        {
            curBar = Instantiate(Bar);
            curBar.transform.parent = curLineChart.transform;
            curBar.transform.localScale = new Vector3(curBar.transform.localScale.x / 100,
                                                       curBar.transform.localScale.y / 100,
                                                        curBar.transform.localScale.z / 100);
            curBar.transform.position = new Vector3(origin.transform.position.x + XOffset + (gapBetweenBars * (i + 1)), origin.transform.position.y + YOffset, origin.transform.position.z);
            curBar.transform.Find("Label").GetComponent<TextMesh>().text = XValues[i];

            float targetHeight = (YValues1[i] / YValueMax) * (YEnd.transform.localPosition.y - 11.58f);

            GameObject curCubeBase = curBar.transform.Find("CubeBase").gameObject;
            GameObject curHeight = curCubeBase.transform.Find("BarHeightPosition").gameObject;
            curHeight.transform.parent = curLineChart.transform;

            //Debug.Log(targetHeight);
            while (curHeight.transform.localPosition.y - 11.58f < targetHeight)
            {

                curHeight.transform.parent = curCubeBase.transform;
                curCubeBase.transform.localScale = new Vector3(curCubeBase.transform.localScale.x,
                                                                curCubeBase.transform.localScale.y + 0.1f,
                                                                curCubeBase.transform.localScale.z);
                curCubeBase.transform.localPosition = new Vector3(curCubeBase.transform.position.x,
                                                                   (curCubeBase.transform.localScale.y / 2) - 0.5f,
                                                                    curCubeBase.transform.position.z);

                curHeight.transform.parent = curLineChart.transform;
            }
            curHeight.transform.parent = curCubeBase.transform;
            BarGraphHeights1.Add(curHeight);
            curBar.transform.Find("CubeBase").gameObject.SetActive(false);
        }

        
        #endregion

        #region Trend 2
        for (int i = 0; i < barCount; i++)
        {
            curBar = Instantiate(Bar);
            curBar.transform.parent = curLineChart.transform;
            curBar.transform.localScale = new Vector3(curBar.transform.localScale.x / 100,
                                                       curBar.transform.localScale.y / 100,
                                                        curBar.transform.localScale.z / 100);
            curBar.transform.position = new Vector3(origin.transform.position.x + XOffset + (gapBetweenBars * (i + 1)), origin.transform.position.y + YOffset, origin.transform.position.z);
            curBar.transform.Find("Label").GetComponent<TextMesh>().text = XValues[i];

            float targetHeight = (YValues2[i] / YValueMax) * (YEnd.transform.localPosition.y - 11.58f);

            GameObject curCubeBase = curBar.transform.Find("CubeBase").gameObject;
            GameObject curHeight = curCubeBase.transform.Find("BarHeightPosition").gameObject;
            curHeight.transform.parent = curLineChart.transform;

            //Debug.Log(targetHeight);
            while (curHeight.transform.localPosition.y - 11.58f < targetHeight)
            {

                curHeight.transform.parent = curCubeBase.transform;
                curCubeBase.transform.localScale = new Vector3(curCubeBase.transform.localScale.x,
                                                                curCubeBase.transform.localScale.y + 0.1f,
                                                                curCubeBase.transform.localScale.z);
                curCubeBase.transform.localPosition = new Vector3(curCubeBase.transform.position.x,
                                                                   (curCubeBase.transform.localScale.y / 2) - 0.5f,
                                                                    curCubeBase.transform.position.z);

                curHeight.transform.parent = curLineChart.transform;
            }
            curHeight.transform.parent = curCubeBase.transform;
            BarGraphHeights2.Add(curHeight);
            curBar.transform.Find("CubeBase").gameObject.SetActive(false);
        }


        #endregion

        #region Line Chart Score Updation
        float score = 0f;
        float scoreSum = 0f;
        float maxPossibleScore = 0f;
        for(int i = 0; i < XValues.Length; i++)
        {
            if (YValues1[i] == 0)
                continue;
            scoreSum = scoreSum + (YValues1[i] - YValues2[i]);
            maxPossibleScore = maxPossibleScore + (YValueMax-YValues2[i]);
        }

        score = scoreSum / maxPossibleScore;
        //scoreSum = scoreSum / XValues.Length;
        curLineChart.transform.Find("Score").Find("ScoreLabel").GetComponent<TextMesh>().text = score.ToString();
        #endregion

    }
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            #region Setting colors for bar graph
            barColors = new List<Color>();
            barColors.Add(Color.red);
            barColors.Add(Color.blue);
            barColors.Add(Color.yellow);
            barColors.Add(Color.gray);
            barColors.Add(Color.green);
            barColors.Add(Color.cyan);
            barColors.Add(Color.white);
            barColors.Add(Color.magenta);
            barColors.Add(Color.grey);
            barColors.Add(Color.black);
            barColors.Add(Color.clear);
            barColors.Add(Color.cyan);
            barColors.Add(new Color(30,60,90));
            barColors.Add(Color.red);
            barColors.Add(Color.cyan);
            barColors.Add(Color.yellow);
            #endregion

            selectedThreshold = 50;
            selectedBoro = "QUEENS";

            allVisibleCubes = new List<RestaurantObject>();
            
            System.Random rand = new System.Random();
            BoroWiseDict = new Dictionary<string, Dictionary<string, RestaurantObject>>();
            BoroWiseDict["MANHATTAN"] = new Dictionary<string, RestaurantObject>();
            BoroWiseDict["BRONX"] = new Dictionary<string, RestaurantObject>();
            BoroWiseDict["STATEN ISLAND"] = new Dictionary<string, RestaurantObject>();
            BoroWiseDict["BROOKLYN"] = new Dictionary<string, RestaurantObject>();
            BoroWiseDict["QUEENS"] = new Dictionary<string, RestaurantObject>();
            BiggestPlane = ImagePlane;


            //test.csv
            //Features = new string[] { "ID", "CAMIS", "DBA", "BORO", "BUILDING", "STREET", "ZIPCODE", "PHONE", "INSPECTIONDATE", "ACTION", "VIOLATIONCODE", "CRITICALFLAG", "SCORE", "GRADE", "GRADEDATE", "RECORDDATE", "INSPECTIONTYPE" };


            //valid_data.csv
            //Features = new string[] { "ID", "CAMIS", "DBA", "BORO", "BUILDING", "STREET", "ZIPCODE", "PHONE", "CUISINEDESCRIPTION", "INSPECTIONDATE", "ACTION", "VIOLATIONCODE", "VIOLATIONDESCRIPTION", "CRITICALFLAG", "SCORE", "GRADE", "GRADEDATE", "RECORDDATE", "INSPECTIONTYPE", "MONTH" };

            //testnkTruncated.csv
            Features = new string[] { "ID", "DBA", "BORO", "STREET", "ZIPCODE", "PHONE", "CUISINEDESCRIPTION", "INSPECTIONDATE", "VIOLATIONDESCRIPTION", "CRITICALFLAG", "SCORE", "GRADE", "MONTH" };

            //dict = new Dictionary<string, List<Restaurant>>();
            //PlacedGameObjects = new List<GameObject>();
            Button btn = FinalizePlaneButton.GetComponent<Button>();
            btn.onClick.AddListener(FinalizePlane);

            Button btn2 = SelectAreaButton.GetComponent<Button>();
            btn2.onClick.AddListener(MoveToSelectedArea);

            Button btn3 = ButtonZoomOut.GetComponent<Button>();
            btn3.onClick.AddListener(SwitchToZoomOutMode);

            Button btn4 = ButtonManhattan.GetComponent<Button>();
            btn4.onClick.AddListener(ApplyManhattanFilter);

            Button btn5 = ButtonBronx.GetComponent<Button>();
            btn5.onClick.AddListener(ApplyBronxFilter);

            Button btn6 = ButtonBrooklyn.GetComponent<Button>();
            btn6.onClick.AddListener(ApplyBrooklynFilter);

            Button btn7 = ButtonQueens.GetComponent<Button>();
            btn7.onClick.AddListener(ApplyQueensFilter);

            Button btn8 = ButtonStatenIsland.GetComponent<Button>();
            btn8.onClick.AddListener(ApplyStatenIslandFilter);

            slider.onValueChanged.AddListener(ChangeThreshold);
            
            using (var reader = new StreamReader(new MemoryStream((Resources.Load("test10kTruncated") as TextAsset).bytes)))
            {
                int rowCount = 0;
                
                while (!reader.EndOfStream)
                {
                    //Processing row
                    string[] fields = reader.ReadLine().Split(',');
                    if (rowCount == 0)
                    {
                        rowCount++;
                        continue;
                    }
                    Restaurant curRestaurant = new Restaurant();
                    int curFeatureIndex = 0;
                    foreach (string field in fields)
                    {
                        curRestaurant.features[Features[curFeatureIndex++]] = field;
                    }
                    RestaurantObject curRestaurantObject = new RestaurantObject();
                    curRestaurantObject.restaurantDetails = curRestaurant;
                    BoroWiseDict[curRestaurant.features["BORO"]]["RES:" + curRestaurant.features["ID"]] = curRestaurantObject;
                    rowCount++;
                }

                ARText.GetComponent<TextMesh>().text = "RESTAURANT COUNT:";
                ARText.GetComponent<TextMesh>().text = ARText.GetComponent<TextMesh>().text + "\nManhattan: " + BoroWiseDict["MANHATTAN"].Count.ToString();
                ARText.GetComponent<TextMesh>().text = ARText.GetComponent<TextMesh>().text + "\nBronx: " + BoroWiseDict["BRONX"].Count.ToString();
                ARText.GetComponent<TextMesh>().text = ARText.GetComponent<TextMesh>().text + "\nQueens: " + BoroWiseDict["QUEENS"].Count.ToString();
                ARText.GetComponent<TextMesh>().text = ARText.GetComponent<TextMesh>().text + "\nBrooklyn: " + BoroWiseDict["BROOKLYN"].Count.ToString();
                ARText.GetComponent<TextMesh>().text = ARText.GetComponent<TextMesh>().text + "\nStaten Island: " + BoroWiseDict["STATEN ISLAND"].Count.ToString();
            }

            CreateAndPlaceCubesOnMap();
        }
        catch (Exception ex)
        {
            ExceptionOccured = true;
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LastExceptionString = "??";
        }
    }

    void CreateAndPlaceCubesOnMap()
    {
        try
        {
            System.Random rand = new System.Random();

            #region Deleting previously visible cubes that don't fit the query.
            List<RestaurantObject> toBeRemoved = new List<RestaurantObject>();
            foreach (RestaurantObject curCubeObject in allVisibleCubes)
            {
                if ((Int32.Parse(curCubeObject.restaurantDetails.features["SCORE"]) < selectedThreshold) || !(curCubeObject.restaurantDetails.features["BORO"].Equals(selectedBoro)))
                {
                    Destroy(curCubeObject.restaurantGameObject);
                    toBeRemoved.Add(curCubeObject);
                    //allVisibleCubes.Remove(curCubeObject);
                }
            }

            foreach (RestaurantObject curCubeObject in toBeRemoved)
            {
                allVisibleCubes.Remove(curCubeObject);
            }

            #endregion

            #region Restaurant 3D code
            foreach (KeyValuePair<string, RestaurantObject> curPair in BoroWiseDict[selectedBoro])
            {
                RestaurantObject curRestaurantObject = curPair.Value;
                Restaurant curRestaurant = curRestaurantObject.restaurantDetails;

                if (Int32.Parse(curRestaurant.features["SCORE"]) > selectedThreshold)
                {
                    if (!allVisibleCubes.Contains(curRestaurantObject))
                    {
                        curRestaurantObject.restaurantGameObject = Instantiate(Restaurant3DGameObject);
                        curRestaurantObject.restaurantGameObject.transform.parent = ImagePlane.transform;
                        curRestaurantObject.restaurantGameObject.transform.localScale = new Vector3(
                            Restaurant3DGameObject.transform.localScale.x,
                            Restaurant3DGameObject.transform.localScale.y + 0.1f,
                            Restaurant3DGameObject.transform.localScale.z);
                        curRestaurantObject.restaurantGameObject.SetActive(true);
                        GameObject targetPlane = BrooklynPlane;
                        if (curRestaurant.features["BORO"].Equals("BROOKLYN"))
                            targetPlane = BrooklynPlane;
                        else if (curRestaurant.features["BORO"].Equals("MANHATTAN"))
                            targetPlane = ManhattanPlane;
                        else if (curRestaurant.features["BORO"].Equals("BRONX"))
                            targetPlane = BronxPlane;
                        else if (curRestaurant.features["BORO"].Equals("STATEN ISLAND"))
                            targetPlane = StatenIslandPlane;
                        else if (curRestaurant.features["BORO"].Equals("QUEENS"))
                            targetPlane = QueensPlane;

                        float randomFactor = 0.2f;
                        if (targetPlane == ManhattanPlane)
                            randomFactor = 0.05f;
                        if (targetPlane == BronxPlane)
                            randomFactor = 0.1f;
                        if (ZoomedIn)
                            randomFactor = 3f;
                        float X_Offset = (float)(rand.NextDouble()) * randomFactor;
                        float Z_Offset = (float)(rand.NextDouble()) * randomFactor;

                        curRestaurantObject.restaurantGameObject.transform.position =
                                                        new Vector3(
                                                                targetPlane.transform.position.x + X_Offset,
                                                                targetPlane.transform.position.y,
                                                                targetPlane.transform.position.z + Z_Offset
                                                            );
                        if (curRestaurant.features["CRITICALFLAG"].Equals("Critical"))
                            curRestaurantObject.restaurantGameObject.transform.Find("3DModel").gameObject.GetComponent<Renderer>().material.color = new Color(100, 0, 0, 0.5f);
                        else
                            curRestaurantObject.restaurantGameObject.transform.Find("3DModel").gameObject.GetComponent<Renderer>().material.color = new Color(0, 100, 0, 50f);

                        curRestaurantObject.restaurantGameObject.transform.Find("3DModel").name = "RES:" + curRestaurant.features["ID"];

                        curRestaurantObject.restaurantGameObject.transform.Find("RestaurantLabel").GetComponent<TextMesh>().text = curRestaurant.features["DBA"];
                        allVisibleCubes.Add(curRestaurantObject);
                    }
                }
            }
            #endregion

            #region Graph Updation Calculations
            int criticalYes = 0;
            int criticalNo = 0;
            Dictionary<int, int> monthCriticalCount = new Dictionary<int, int>();
            Dictionary<int, int> monthInspectionCount = new Dictionary<int, int>();
            Dictionary<string, int> cuisineCount = new Dictionary<string, int>();
            for (int i = 1; i <= 12; i++)
            {
                monthCriticalCount[i] = 0;
                monthInspectionCount[i] = 0;
            }
            foreach (RestaurantObject curCubeObject in allVisibleCubes)
            {
                if (curCubeObject.restaurantDetails.features["CRITICALFLAG"].Equals("Critical"))
                {
                    criticalYes++;
                    monthCriticalCount[Int32.Parse(curCubeObject.restaurantDetails.features["MONTH"])]++;
                }
                else
                    criticalNo++;

                //Debug.Log("1");
                monthInspectionCount[Int32.Parse(curCubeObject.restaurantDetails.features["MONTH"])]++;
                //Debug.Log("2");
                if (cuisineCount.ContainsKey(curCubeObject.restaurantDetails.features["CUISINEDESCRIPTION"]))
                {
                    //Debug.Log("3");
                    cuisineCount[curCubeObject.restaurantDetails.features["CUISINEDESCRIPTION"]]++;
                    //Debug.Log("4");
                }
                else
                    cuisineCount[curCubeObject.restaurantDetails.features["CUISINEDESCRIPTION"]] = 1;

            }

            List<CuisineCount> CuisineCountList = new List<CuisineCount>();
            foreach (KeyValuePair<string, int> curCuisine in cuisineCount)
            {
                CuisineCount curCuisineCount = new CuisineCount();
                curCuisineCount.Count = curCuisine.Value;
                curCuisineCount.Name = curCuisine.Key;
                CuisineCountList.Add(curCuisineCount);
            }
            //Debug.Log("5");
            CuisineCountList.Sort((x, y) => y.Count - x.Count);

            int CuisineGraphCount = 15;
            string[] CuisineXLabels = new string[CuisineGraphCount];
            float[] CuisineYLabels = new float[CuisineGraphCount];
            if (CuisineCountList.Count < 15)
                CuisineGraphCount = CuisineCountList.Count;
            int labelCount = 0;
            for (int i = 0; i < CuisineGraphCount - 1; i++)
            {
                if (CuisineCountList[i].Count == 0)
                    continue;
                CuisineXLabels[labelCount] = CuisineCountList[i].Name;
                CuisineYLabels[labelCount] = CuisineCountList[i].Count;
                labelCount++;
            }

            CuisineXLabels[CuisineGraphCount - 1] = "Others";
            CuisineYLabels[CuisineGraphCount - 1] = 0f;

            for (int i = CuisineGraphCount - 1; i < CuisineCountList.Count; i++)
            {
                CuisineYLabels[CuisineGraphCount - 1] = CuisineYLabels[CuisineGraphCount - 1] + CuisineCountList[i].Count;
            }

            //Removing zero count cuisines
            string[] CuisineXLabelsCleaned = new string[labelCount];
            float[] CuisineYLabelsCleaned = new float[labelCount];
            for (int i = 0; i < labelCount; i++)
            {
                CuisineXLabelsCleaned[i] = CuisineXLabels[i];
                CuisineYLabelsCleaned[i] = CuisineYLabels[i];
            }


            //Debug.Log("8");


            CreateBarGraph(CuisineXLabelsCleaned, CuisineYLabelsCleaned, CuisineYLabelsCleaned[0], "Cuisines", "Restaurant Count");
            //CreateBarGraph(new string[] { "Critical", "Not Critical"}, new float[] { criticalYes, criticalNo }, criticalYes + criticalNo);

            string[] monthsLabels = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            float[] InspectionTrend = new float[12];
            float[] CriticalTrend = new float[12];
            for (int i = 1; i <= 12; i++)
            {
                if (monthInspectionCount.ContainsKey(i))
                    InspectionTrend[i - 1] = monthInspectionCount[i];
                if (monthCriticalCount.ContainsKey(i))
                    CriticalTrend[i - 1] = monthCriticalCount[i];

            }

            CreateLineChart(monthsLabels, InspectionTrend, CriticalTrend, "Months", "Count");
            #endregion
        }
        catch(Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LastExceptionString = "ex! cube function!";
            ExceptionOccured = true;
        }
    }
    void UpdateLineChartPositions()
    {
        curLineChart.transform.rotation = new Quaternion(0, 0, 0, 0);
        int barCount = 12;
        Trend1LineRenderer = curLineChart.transform.Find("Trend1").GetComponent<LineRenderer>();
        Trend1LineRenderer.positionCount = barCount;
        Trend1LineRenderer.material = new Material(Shader.Find("Standard"));
        Trend1LineRenderer.startColor = Color.red;
        Trend1LineRenderer.endColor = Color.red;
        Trend1LineRenderer.startWidth = 0.003f;
        for (int i = 0; i < barCount; i++)
        {
            curBar = BarGraphHeights1[i];
            Trend1LineRenderer.SetPosition(i, curBar.transform.position);
        }

        Trend2LineRenderer = curLineChart.transform.Find("Trend2").GetComponent<LineRenderer>();
        Trend2LineRenderer.positionCount = barCount;
        Trend2LineRenderer.startWidth = 0.003f;
        for (int i = 0; i < barCount; i++)
        {
            curBar = BarGraphHeights2[i];
            Trend2LineRenderer.SetPosition(i, curBar.transform.position);
        }
    }
    // Update is called once per frame
    void Update()
    {

        try
        {

            UpdateLineChartPositions();
            if(curBarGraph != null)
            {
                curBarGraph.transform.position = BarGraph.transform.position;
                curBarGraph.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
            if(curLineChart != null)
            {
                curLineChart.transform.position = LineChart.transform.position;
                curLineChart.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
         
            if (ModeSelection)
            {
                if (Input.touchCount > 0)
                {
                    if (EventSystem.current.IsPointerOverGameObject(0))    // is the touch on the GUI
                    {
                        return;
                    }
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit Hit;

                    if (Physics.Raycast(ray, out Hit, 20.0f))
                    {
                        if ((Hit.transform.name.StartsWith("BrooklynCollider") || (Hit.transform.name.StartsWith("StatenIslandCollider")) || (Hit.transform.name.StartsWith("ManhattanCollider")) || (Hit.transform.name.StartsWith("QueensCollider")) || (Hit.transform.name.StartsWith("BronxCollider"))))
                        {
                            currentObject = Hit.transform.gameObject;
                        }

                        if (Hit.transform.name.StartsWith("BrooklynCollider"))
                        {
                            currentBoro = BrooklynPlane;
                            curBoroRestaurantDict = BoroWiseDict["BROOKLYN"];
                        }
                        else if (Hit.transform.name.StartsWith("StatenIslandCollider"))
                        {
                            currentBoro = StatenIslandPlane;
                            curBoroRestaurantDict = BoroWiseDict["STATEN ISLAND"];
                        }
                        else if (Hit.transform.name.StartsWith("BronxCollider"))
                        {
                            currentBoro = BronxPlane;
                            curBoroRestaurantDict = BoroWiseDict["BRONX"];
                        }
                        else if (Hit.transform.name.StartsWith("ManhattanCollider"))
                        {
                            currentBoro = ManhattanPlane;
                            curBoroRestaurantDict = BoroWiseDict["MANHATTAN"];
                        }
                        else if (Hit.transform.name.StartsWith("QueensCollider"))
                        {
                            currentBoro = QueensPlane;
                            curBoroRestaurantDict = BoroWiseDict["QUEENS"];
                        }
                    }
                }
            }
            else if (InTransitionZoomIn)
            {
                TransitionMapTowardsCamera();
            }
            else if (InTransitionZoomOut)
            {
                ZoomOut();
            }
            else if (ModeBuild)
            {
               

                if (currentObject == null)
                    ARText.GetComponent<TextMesh>().text = "Select a restaurant!";

                if (Input.touchCount > 0)
                {
                    if (EventSystem.current.IsPointerOverGameObject(0))    // is the touch on the GUI
                    {
                        // GUI Action
                        //ARText.GetComponent<TextMesh>().text = "UI TOUCH!";
                        return;
                    }
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit Hit;

                    if (Physics.Raycast(ray, out Hit, 20.0f))
                    {
                        currentObject = Hit.transform.gameObject;
                        if (Hit.transform.name.StartsWith("RES:"))
                        {
                            //ARAreaText.GetComponent<TextMesh>().text = "Selected Area: " + Hit.transform.name;
                            RestaurantObject restaurantObject = null;
                            if (currentBoro == BrooklynPlane)
                                restaurantObject = BoroWiseDict["BROOKLYN"][Hit.transform.name];
                            else if (currentBoro == StatenIslandPlane)
                                restaurantObject = BoroWiseDict["STATEN ISLAND"][Hit.transform.name];
                            else if (currentBoro == BronxPlane)
                                restaurantObject = BoroWiseDict["BRONX"][Hit.transform.name];
                            else if (currentBoro == QueensPlane)
                                restaurantObject = BoroWiseDict["QUEENS"][Hit.transform.name];
                            else if (currentBoro == ManhattanPlane)
                                restaurantObject = BoroWiseDict["MANHATTAN"][Hit.transform.name];
                            restaurantObject.restaurantGameObject.GetComponent<Light>().enabled = true;
                            restaurantObject.restaurantGameObject.GetComponent<Light>().range = restaurantObject.restaurantGameObject.GetComponent<Light>().range + HaloChange;
                            restaurantObject.restaurantGameObject.GetComponent<Light>().intensity = restaurantObject.restaurantGameObject.GetComponent<Light>().intensity + HaloChange;

                            //if ((restaurantObject.restaurantGameObject.GetComponent<Light>().range > 0.1) || (restaurantObject.restaurantGameObject.GetComponent<Light>().range < 0.01))
                            //{
                            //    HaloChange = HaloChange * -1;
                            //}

                            ARText.GetComponent<TextMesh>().text = "Restaurant Details: ";
                            ARText.GetComponent<TextMesh>().text = ARText.GetComponent<TextMesh>().text + "\nName: " + restaurantObject.restaurantDetails.features["DBA"];
                            ARText.GetComponent<TextMesh>().text = ARText.GetComponent<TextMesh>().text + "\nPhone: " + restaurantObject.restaurantDetails.features["PHONE"];
                            ARText.GetComponent<TextMesh>().text = ARText.GetComponent<TextMesh>().text + "\nCriticality: " + restaurantObject.restaurantDetails.features["CRITICALFLAG"];
                            ARText.GetComponent<TextMesh>().text = ARText.GetComponent<TextMesh>().text + "\nGrade: " + restaurantObject.restaurantDetails.features["GRADE"];
                            ARText.GetComponent<TextMesh>().text = ARText.GetComponent<TextMesh>().text + "\nScore: " + restaurantObject.restaurantDetails.features["SCORE"];
                            ARText.GetComponent<TextMesh>().text = ARText.GetComponent<TextMesh>().text + "\nCuisines: " + restaurantObject.restaurantDetails.features["CUISINEDESCRIPTION"];
                            


                        }
                    }
                }

            }

            float newX = BiggestPlane.transform.position.x + LeftJoystick.Horizontal / 50;
            float newZ = BiggestPlane.transform.position.z + LeftJoystick.Vertical / 50;
            BiggestPlane.transform.position = new Vector3(newX, BiggestPlane.transform.position.y, newZ);
            BiggestPlane.transform.Rotate(new Vector3(0, RightJoystick.Horizontal, 0));

            if (currentObject != null)
                ARAreaText.GetComponent<TextMesh>().text = "Selected Area: " + selectedBoro;
        }
        catch (Exception ex)
        {
            ExceptionOccured = true;
            
            UnityEngine.Debug.Log("EXCEPTION!");
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LastExceptionString = line.ToString();
        }
        //if (ExceptionOccured)
        //    ARText.GetComponent<TextMesh>().text = "Exception update!: " + LastExceptionString;
    }
}