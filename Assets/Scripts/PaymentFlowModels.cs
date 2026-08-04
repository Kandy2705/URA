using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class WalletBillDefinition
{
    public int denomination;
    public int count;

    public WalletBillDefinition()
    {
    }

    public WalletBillDefinition(int denomination, int count)
    {
        this.denomination = denomination;
        this.count = count;
    }
}

[Serializable]
public class WalletBillSnapshot
{
    public int denomination;
    public int count;
    public int subtotal;

    public WalletBillSnapshot()
    {
    }

    public WalletBillSnapshot(int denomination, int count)
    {
        this.denomination = denomination;
        this.count = count;
        subtotal = denomination * count;
    }
}

[Serializable]
public class PaymentSummary
{
    public int requiredAmount;
    public int paidAmount;
    public int differenceAmount;
    public string resultCode;
    public string note;
    public List<WalletBillSnapshot> startingWallet = new List<WalletBillSnapshot>();
    public List<WalletBillSnapshot> submittedBills = new List<WalletBillSnapshot>();

    public static PaymentSummary Create(
        int requiredAmount,
        int paidAmount,
        IEnumerable<WalletBillSnapshot> startingWallet,
        IEnumerable<WalletBillSnapshot> submittedBills)
    {
        PaymentSummary summary = new PaymentSummary
        {
            requiredAmount = requiredAmount,
            paidAmount = paidAmount,
            differenceAmount = paidAmount - requiredAmount,
            startingWallet = startingWallet != null
                ? startingWallet.Select(snapshot => new WalletBillSnapshot(snapshot.denomination, snapshot.count)).ToList()
                : new List<WalletBillSnapshot>(),
            submittedBills = submittedBills != null
                ? submittedBills.Select(snapshot => new WalletBillSnapshot(snapshot.denomination, snapshot.count)).ToList()
                : new List<WalletBillSnapshot>()
        };

        if (summary.differenceAmount == 0)
        {
            summary.resultCode = "EXACT";
            summary.note = "Tra du tien";
        }
        else if (summary.differenceAmount < 0)
        {
            summary.resultCode = "UNDERPAID";
            summary.note = $"Thieu {Math.Abs(summary.differenceAmount):N0} VND";
        }
        else
        {
            summary.resultCode = "OVERPAID";
            summary.note = $"Du {summary.differenceAmount:N0} VND";
        }

        return summary;
    }
}
