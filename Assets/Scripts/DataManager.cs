using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Linq;

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
        // Application.dataPath trỏ vào vùng dữ liệu của APK trên Android/Quest,
        // thường là read-only. persistentDataPath là thư mục dành cho dữ liệu
        // phát sinh khi chạy app và có thể ghi được trên thiết bị.
        string dirPath = Path.Combine(Application.persistentDataPath, "Data");
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

        string path = Path.Combine(dirPath, fileName);
        Debug.Log($"[DataManager] CSV report saved at: {path}");
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            writer.WriteLine("Booth Type,Visit Count");
            writer.WriteLine($"Fruits,{num_visit_fruits}");
            writer.WriteLine($"Drinks,{num_visit_drinks}");
            writer.WriteLine($"Snacks,{num_visit_snacks}");
            writer.WriteLine();

            writer.WriteLine("Visit Order of Booths");
            writer.WriteLine(string.Join(" -> ", booths_priority));
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
            writer.WriteLine("Total Items," + grandTotalItems);
            writer.WriteLine("Total Price," + grandTotalPrice);
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
            writer.WriteLine("Show list ," + click_num + " time" + (click_num <= 1 ? "" : "s"));
        }

        TryExportToPublicDownloads(path, fileName);
    }

    private static void TryExportToPublicDownloads(string sourcePath, string fileName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Android 10+ requires MediaStore to write into the public Downloads folder.
        try
        {
            using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                       .GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject resolver = activity.Call<AndroidJavaObject>("getContentResolver"))
            using (AndroidJavaClass downloads = new AndroidJavaClass("android.provider.MediaStore$Downloads"))
            using (AndroidJavaObject downloadsUri = downloads.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI"))
            using (AndroidJavaObject values = new AndroidJavaObject("android.content.ContentValues"))
            {
                values.Call("put", "_display_name", fileName);
                values.Call("put", "mime_type", "text/csv");
                values.Call("put", "relative_path", "Download/URA/");

                using (AndroidJavaObject uri = resolver.Call<AndroidJavaObject>("insert", downloadsUri, values))
                {
                    if (uri == null)
                    {
                        Debug.LogWarning("[DataManager] Không thể tạo file CSV trong thư mục Downloads.");
                        return;
                    }

                    using (AndroidJavaObject outputStream = resolver.Call<AndroidJavaObject>("openOutputStream", uri))
                    {
                        outputStream.Call("write", File.ReadAllBytes(sourcePath));
                        outputStream.Call("flush");
                    }
                }
            }

            Debug.Log($"[DataManager] CSV copied to Quest Downloads/URA/{fileName}");
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning($"[DataManager] Không thể export CSV ra Downloads: {exception.Message}");
        }
#else
        Debug.Log($"[DataManager] Public Downloads export chỉ chạy trên Android/Quest. File gốc: {sourcePath}");
#endif
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
