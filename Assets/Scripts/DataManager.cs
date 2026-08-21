using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Linq;
using TMPro.Examples;

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
    private List<CompareResult> compareResults = new List<CompareResult>();
    private PaymentSummary paymentSummary;
    private List<int> submitted_amounts_history = new List<int>();
    private List<int> difference_amounts_history = new List<int>();

    public void AddCompareResult(CompareResult result)
    {
        compareResults.Add(result);
    }

    public void SetCompareResults(IEnumerable<CompareResult> results)
    {
        compareResults = results != null
            ? new List<CompareResult>(results)
            : new List<CompareResult>();
    }

    public void SetPaymentSummary(PaymentSummary summary)
    {
        paymentSummary = summary;
    }

    public void SetPaymentHistory(List<int> submittedAmountsHistory, List<int> differenceAmountsHistory)
    {
        submitted_amounts_history = submittedAmountsHistory;
        difference_amounts_history = differenceAmountsHistory;
    }

    public Transform player;          // Player transform
    public Transform[] targets;       // Assign 3 targets in the Inspector
    public float interactionRange = 10f;
    public GameObject boardData;

    public TMP_Text statsText1;   // Drag your StatsText UI element here
    public TMP_Text statsText2;

    int num_visit_fruits = 0;
    int num_visit_drinks = 0;
    int num_visit_snacks = 0;

    private bool[] isInside;
    private List<string> booths_priority = new List<string>();

    private float startTime;
    Dictionary<string, float> product_times = new Dictionary<string, float>();

    [SerializeField] GameObject target;

    private void Start()
    {
        startTime = Time.time;
        isInside = new bool[targets.Length];

        if (boardData != null)
            boardData.SetActive(false);
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
        product_times[product] = elapsed;
    }

    public void ExportCSV(List<CompareResult> compareResults)
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"report_{timestamp}.csv";
        string dirPath = Path.Combine(Application.dataPath, "Scripts/Data");
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

        string path = Path.Combine(dirPath, fileName);
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            writer.WriteLine($"Participant ID,{GameSessionContext.Instance.citizenId}");
            writer.WriteLine($"Level,{GameSessionContext.Instance.level}");

            writer.WriteLine("Booth Type,Visit Count");
            writer.WriteLine($"Fruits,{num_visit_fruits}");
            writer.WriteLine($"Drinks,{num_visit_drinks}");
            writer.WriteLine($"Snacks,{num_visit_snacks}");
            writer.WriteLine();

            writer.WriteLine("Visit Order of Booths");
            writer.WriteLine(string.Join(" -> ", booths_priority));
            writer.WriteLine();


            int click_num = -1;
            // GameObject target = GameObject.Find("Supermarket/Notice_Board/UI Sample/Scroll UI Sample");
            if (target == null)
            {
                Debug.Log("KHÔNG TÌM THẤY GameObject Scroll UI Sample");
            }
            else
            {
                ListController listCtrlr = target.GetComponent<ListController>();
                if (listCtrlr == null)
                {
                    Debug.Log("KHÔNG TÌM THẤY Component của Object");
                }
                else
                {
                    click_num = listCtrlr.GetClickNumber();
                }
            }
            writer.WriteLine($"Show list {click_num} time{(click_num == 1 ? ' ' : 's')}");
            writer.WriteLine();

            writer.WriteLine("Product,Time (s)");
            foreach (var kvp in product_times)
            {
                writer.WriteLine($"{kvp.Key},{kvp.Value}");
            }
            writer.WriteLine();

            writer.WriteLine("Product Name,Total Count,Unit Price,Status,Subtotal");

            Dictionary<string, CompareResult> finalResults =
                new Dictionary<string, CompareResult>();

            foreach (var r in compareResults)
            {
                finalResults[r.itemName] = r;
            }

            int grandTotalItems = 0;
            long grandTotalPrice = 0;

            foreach (var kvp in finalResults)
            {
                CompareResult r = kvp.Value;

                int count = r.currentQuantity;
                int unitPrice = r.price;
                long subtotal = (long)count * unitPrice;

                writer.WriteLine($"{r.itemName},{count},{unitPrice},{r.status},{subtotal}");

                grandTotalItems += count;
                grandTotalPrice += subtotal;
            }

            writer.WriteLine();

            writer.WriteLine("Bill Item,Quantity,Unit Price,Subtotal");
            if (CartManager.Instance != null && CartManager.Instance.bill != null)
            {
                foreach (BillEntry entry in CartManager.Instance.bill.Values.OrderBy(item => item.itemName))
                {
                    if (entry == null)
                        continue;

                    long subtotal = (long)entry.price * entry.quantity;
                    writer.WriteLine($"{EscapeCsv(entry.itemName)},{entry.quantity},{entry.price},{subtotal}");
                }
            }

            writer.WriteLine();

            writer.WriteLine("Total Items," + grandTotalItems);
            writer.WriteLine("Total Price," + grandTotalPrice);
            writer.WriteLine();

            writer.WriteLine("Payment Summary");
            writer.WriteLine("Required Amount,Paid Amount,Difference,Result,Note");
            if (paymentSummary != null)
            {
                writer.WriteLine(
                    $"{paymentSummary.requiredAmount},{paymentSummary.paidAmount},{paymentSummary.differenceAmount}," +
                    $"{paymentSummary.resultCode},{EscapeCsv(paymentSummary.note)}");

                writer.WriteLine();
                writer.WriteLine("Starting Wallet");
                writer.WriteLine("Denomination,Count,Subtotal");
                foreach (WalletBillSnapshot snapshot in paymentSummary.startingWallet)
                {
                    writer.WriteLine($"{snapshot.denomination},{snapshot.count},{snapshot.subtotal}");
                }

                writer.WriteLine();
                writer.WriteLine("Submitted Bills");
                writer.WriteLine("Denomination,Count,Subtotal");
                foreach (WalletBillSnapshot snapshot in paymentSummary.submittedBills)
                {
                    writer.WriteLine($"{snapshot.denomination},{snapshot.count},{snapshot.subtotal}");
                }
            }
            else
            {
                writer.WriteLine("0,0,0,NOT_CAPTURED,Payment summary not available");
            }
            writer.WriteLine();

            if(submitted_amounts_history != null && difference_amounts_history != null){
                int correct_money_count = submitted_amounts_history.Count;
                writer.WriteLine($"Correct Money Count,{correct_money_count}");
                writer.WriteLine($"Submitted Amounts History,{string.Join("," , submitted_amounts_history)}");
                writer.WriteLine($"Difference Amounts History,{string.Join("," , difference_amounts_history)}");
            }else{
                writer.WriteLine("Fail to record Submit history !");
            }
        }
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        bool needsQuotes = value.Contains(",") || value.Contains("\"") || value.Contains("\n");
        string escaped = value.Replace("\"", "\"\"");
        return needsQuotes ? $"\"{escaped}\"" : escaped;
    }






    public void Report()
    {
        // Update UI text
        statsText1.text =
            "Số lần ghé quầy trái cây: " + num_visit_fruits + "\n" +
            "Số lần ghé quầy nước uống: " + num_visit_drinks + "\n" +
            "Số lần ghé quầy bánh kẹo: " + num_visit_snacks + "\n" +
            "Thứ tự ghé các quầy: " + string.Join(" → ", booths_priority) + "\n";

        statsText2.text = "\n Thời điểm lấy sản phẩm:\n";

        foreach (var kvp in product_times)
        {
            statsText2.text += $"{kvp.Key}: {kvp.Value:F2}s\n";
        }

        // Show the stats panel
        EnableStatsPanel();
        ExportCSV(compareResults);
    }

    public void EnableStatsPanel()
    {
        if (boardData != null)
            boardData.SetActive(false);
    }
}
