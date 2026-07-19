using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class MllmApiSendGate
{
    public struct Evaluation
    {
        public bool canSend;
        public string skipReason;
        public string fingerprint;
    }

    private float _lastSendTime = -9999f;
    private string _lastFingerprint;

    public float MinimumSendIntervalSeconds { get; set; } = 4f;
    public float VoiceChatMinimumIntervalSeconds { get; set; } = 2f;
    public bool LogSkips { get; set; } = true;

    public Evaluation Evaluate(MllmGenerateDialogueRequest request, bool isRequestInFlight)
    {
        if (request == null)
            return Block("request_null");

        if (request.content == null)
            return Block("content_null");

        if (string.IsNullOrWhiteSpace(request.citizen_id))
            return Block("missing_citizen_id");

        if (isRequestInFlight)
            return Block("request_in_flight");

        string eventCode = request.content.event_code ?? string.Empty;
        float minInterval = eventCode == MllmEventCodes.VoiceChatTrigger
            ? VoiceChatMinimumIntervalSeconds
            : MinimumSendIntervalSeconds;

        float elapsed = Time.unscaledTime - _lastSendTime;
        if (elapsed < minInterval)
            return Block($"cooldown_active ({elapsed:F1}s < {minInterval:F1}s)");

        string fingerprint = BuildFingerprint(request);
        if (!string.IsNullOrEmpty(_lastFingerprint) &&
            string.Equals(fingerprint, _lastFingerprint, StringComparison.Ordinal))
        {
            return Block("duplicate_payload");
        }

        return new Evaluation
        {
            canSend = true,
            fingerprint = fingerprint
        };
    }

    public void MarkSendStarted(string fingerprint)
    {
        _lastSendTime = Time.unscaledTime;
        _lastFingerprint = fingerprint;
    }

    public void Reset()
    {
        _lastFingerprint = null;
        _lastSendTime = -9999f;
    }

    public static string BuildFingerprint(MllmGenerateDialogueRequest request)
    {
        if (request == null)
            return string.Empty;

        JObject normalizedContext = NormalizeContextData(request.content?.context_data);
        var payload = new
        {
            request.citizen_id,
            request.level,
            request.game_phase,
            event_code = request.content?.event_code,
            event_details = request.content?.event_details,
            context_data = normalizedContext
        };

        return JsonConvert.SerializeObject(payload, Formatting.None);
    }

    private static JObject NormalizeContextData(JObject contextData)
    {
        if (contextData == null)
            return null;

        JObject copy = (JObject)contextData.DeepClone();

        copy.Remove("list_view_count");
        copy.Remove("scene_name");

        if (copy["limit_seconds"] != null)
        {
            if (float.TryParse(copy["limit_seconds"].ToString(), out float limitSeconds))
                copy["limit_seconds"] = Mathf.RoundToInt(limitSeconds);
        }

        if (copy["total_paid"] is JValue totalPaidValue &&
            int.TryParse(totalPaidValue.ToString(), out int totalPaid))
        {
            int bucket = Mathf.RoundToInt(totalPaid / 1000f) * 1000;
            copy["total_paid"] = bucket;
        }

        if (copy["map_layout"] is JArray mapLayout)
        {
            JArray simplifiedLayout = new JArray();
            foreach (JToken zoneToken in mapLayout)
            {
                if (zoneToken is not JObject zone)
                    continue;

                simplifiedLayout.Add(new JObject
                {
                    ["zone_name"] = zone["zone_name"]?.ToString() ?? string.Empty
                });
            }

            copy["map_layout"] = simplifiedLayout;
        }

        // Postman / backend: current_shopping_list (legacy alias shopping_list vẫn normalize)
        NormalizeShoppingListArray(copy, "current_shopping_list");
        NormalizeShoppingListArray(copy, "shopping_list");

        return copy;
    }

    private static void NormalizeShoppingListArray(JObject copy, string fieldName)
    {
        if (copy[fieldName] is not JArray shoppingList)
            return;

        JArray simplifiedList = new JArray();
        foreach (JToken itemToken in shoppingList)
        {
            if (itemToken is not JObject item)
                continue;

            simplifiedList.Add(new JObject
            {
                ["item_name"] = item["item_name"]?.ToString() ?? string.Empty,
                ["quantity"] = item["quantity"]?.ToString() ?? string.Empty,
                ["unit"] = item["unit"]?.ToString() ?? string.Empty,
                ["unit_price_vnd"] = item["unit_price_vnd"]?.ToString() ?? "0"
            });
        }

        copy[fieldName] = simplifiedList;
    }

    private Evaluation Block(string reason)
    {
        if (LogSkips)
            Debug.LogWarning($"[MllmApiSendGate] Skip API send — {reason}");

        return new Evaluation
        {
            canSend = false,
            skipReason = reason
        };
    }
}