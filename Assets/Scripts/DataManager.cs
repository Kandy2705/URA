using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public Transform player;          // Player transform
    public Transform[] targets;       // Assign 3 targets in the Inspector
    public float interactionRange = 10f;
    public GameObject statsPanel1; // Drag your StatsPanel UI element here
    public GameObject statsPanel2; // Drag your StatsPanel UI element here

    public TMP_Text statsText1;   // Drag your StatsText UI element here
    public TMP_Text statsText2;

    int num_visit_fruits = 0;
    int num_visit_drinks = 0;
    int num_visit_snacks = 0;

    private bool[] isInside;
    private List<string> booths_priority = new List<string>();

    private float startTime;
    Dictionary<string, int> product_times = new Dictionary<string, int>();

    private void Start()
    {
        isInside = new bool[targets.Length];

        if (statsPanel1 != null)
            statsPanel1.SetActive(false);
        if (statsPanel2 != null)
            statsPanel2.SetActive(false);
    }

    private void Update()
    {
        if (player == null || targets.Length == 0) return;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null) continue;

            float dist = Vector3.Distance(player.position, targets[i].position);
            bool withinRange = dist <= interactionRange;

            // Use switch to handle each target differently
            if (withinRange && !isInside[i])
            {
                isInside[i] = true;

                if (i == 0)
                {
                    num_visit_fruits++;
                    Debug.Log("Visited fruits booth! Total: " + num_visit_fruits);
                    booths_priority.Add("Fruits");
                }
                else if (i == 1 || i == 2)
                {
                    num_visit_drinks++;
                    Debug.Log("Visited drinks booth! Total: " + num_visit_drinks);
                    booths_priority.Add("Drinks");
                }
                else if (i == 3)
                {
                    num_visit_snacks++;
                    Debug.Log("Visited snacks booth! Total: " + num_visit_snacks);
                    booths_priority.Add("Snacks");
                }

            }
            // Player just left
            else if (!withinRange && isInside[i])
            {
                isInside[i] = false;
            }
        }
    }

    public void updateTime(string product)
    {
        float elapsed = Time.time - startTime;
        product_times[product] = (int)elapsed;
    }

    public void ExportCSV()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"report_{timestamp}.csv";
        string dirPath = Path.Combine(Application.dataPath, "Scripts/Data");
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);


        string path = Path.Combine(dirPath, fileName);
         using (StreamWriter writer = new StreamWriter(path, false))
    {
        writer.WriteLine("Category,Count");
        writer.WriteLine($"Fruits,{num_visit_fruits}");
        writer.WriteLine($"Drinks,{num_visit_drinks}");
        writer.WriteLine($"Snacks,{num_visit_snacks}");
        writer.WriteLine();

        writer.WriteLine("Booth Order");
        writer.WriteLine(string.Join(" -> ", booths_priority));
        writer.WriteLine();

        writer.WriteLine("Product,Time(s)");
        foreach (var kvp in product_times)
        {
            writer.WriteLine($"{kvp.Key},{kvp.Value}");
        }
    }

    }

    public void Report()
    {
        // Update UI text
        statsText1.text =
            "🍎 Fruits visits: " + num_visit_fruits + "\n" +
            "🥤 Drinks visits: " + num_visit_drinks + "\n" +
            "🍫 Snacks visits: " + num_visit_snacks + "\n\n" +
            "Booth order: " + string.Join(" → ", booths_priority) + "\n";

        statsText2.text = "\n⏱️ Product times:\n";

        foreach (var kvp in product_times)
        {
            statsText2.text += $"{kvp.Key} taken at {kvp.Value:F2}s\n";
        }

        // Show the stats panel
        EnableStatsPanel();
        ExportCSV();

    }

    public void EnableStatsPanel()
    {
        if (statsPanel1 != null)
            statsPanel1.SetActive(true);
        if (statsPanel2 != null)
            statsPanel2.SetActive(true);
    }

    
}
