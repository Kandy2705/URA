using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Request body POST /api/v1/dialogue/generate_refactored — khớp AL-SERVICE Postman.
/// context_data field names: map_layout, current_shopping_list, time_rules, ui_rules,
/// priorities, caller_info, prospective_memory_task, out_of_stock_list, shifting_task,
/// discount_items, solicitor_npcs, decision_rule, hidden_task_results, ...
/// </summary>
[Serializable]
public class MllmGenerateDialogueRequest
{
    [JsonProperty("citizen_id")]
    public string citizen_id;

    [JsonProperty("level")]
    public int level;

    [JsonProperty("game_phase")]
    public string game_phase;

    [JsonProperty("content")]
    public MllmContentData content;

    [JsonProperty("image_base64")]
    public string image_base64;
}

[Serializable]
public class MllmContentData
{
    [JsonProperty("event_code")]
    public string event_code;

    [JsonProperty("event_details")]
    public string event_details;

    [JsonProperty("context_data")]
    public JObject context_data;
}

/// <summary>
/// Response generate_refactored — Unity bắt buộc có result; action/dialogue dùng để NPC nói + anim.
/// </summary>
[Serializable]
public class MllmGenerateDialogueResponse
{
    [JsonProperty("appointment_uid")]
    public string appointment_uid;

    [JsonProperty("result")]
    public MllmAgentResult result;

    [JsonProperty("latency_seconds")]
    public double latency_seconds;

    [JsonProperty("model_latency_seconds")]
    public double model_latency_seconds;
}

[Serializable]
public class MllmAgentResult
{
    [JsonProperty("reasoning")]
    public string reasoning;

    [JsonProperty("action")]
    public string action;

    [JsonProperty("dialogue")]
    public string dialogue;

    [JsonProperty("dialogue_map")]
    public Dictionary<string, string> dialogue_map;
}

/// <summary>Response POST /auth/login</summary>
[Serializable]
public class MllmTokenResponse
{
    [JsonProperty("access_token")]
    public string access_token;

    [JsonProperty("token_type")]
    public string token_type;

    [JsonProperty("expires_in")]
    public int expires_in;

    [JsonProperty("username")]
    public string username;
}

public class MllmApiCallResult
{
    public bool success;
    public int statusCode;
    public string errorMessage;
    public string rawBody;
    public MllmGenerateDialogueResponse response;
    public bool usedFallback;
    public bool isTimeout;
    public bool isNetworkError;
    public bool wasSkipped;
    public string skipReason;

    public static MllmApiCallResult FromSuccess(MllmGenerateDialogueResponse response, string rawBody, int statusCode)
    {
        return new MllmApiCallResult
        {
            success = true,
            statusCode = statusCode,
            response = response,
            rawBody = rawBody
        };
    }

    public static MllmApiCallResult FromError(int statusCode, string errorMessage, string rawBody = null)
    {
        return new MllmApiCallResult
        {
            success = false,
            statusCode = statusCode,
            errorMessage = errorMessage,
            rawBody = rawBody
        };
    }

    public static MllmApiCallResult FromSkip(string skipReason)
    {
        return new MllmApiCallResult
        {
            success = false,
            statusCode = 0,
            wasSkipped = true,
            skipReason = skipReason,
            errorMessage = skipReason
        };
    }
}