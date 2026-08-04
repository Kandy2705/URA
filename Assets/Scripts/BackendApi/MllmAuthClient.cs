using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class MllmAuthClient : MonoBehaviour
{
    public IEnumerator Login(
        BackendApiConfig config,
        string username,
        string password,
        Action<MllmTokenResponse> onSuccess,
        Action<int, string> onError)
    {
        if (config == null)
        {
            onError?.Invoke(0, "BackendApiConfig is null.");
            yield break;
        }

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            onError?.Invoke(0, "Username/password không được để trống khi login.");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest request = UnityWebRequest.Post(config.LoginUrl, form))
        {
            request.timeout = Mathf.CeilToInt(config.timeoutSeconds);
            yield return request.SendWebRequest();

            string body = request.downloadHandler?.text;
            int statusCode = (int)request.responseCode;

            if (request.result != UnityWebRequest.Result.Success)
            {
                string message = BuildHttpErrorMessage(statusCode, request.error, body);
                Debug.LogError($"[MllmAuthClient] Login failed ({statusCode}): {message}");
                onError?.Invoke(statusCode, message);
                yield break;
            }

            try
            {
                MllmTokenResponse tokenResponse = JsonConvert.DeserializeObject<MllmTokenResponse>(body);
                if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.access_token))
                {
                    onError?.Invoke(statusCode, "Login response không có access_token.");
                    yield break;
                }

                Debug.Log($"[MllmAuthClient] Login thành công cho user '{tokenResponse.username}'.");
                onSuccess?.Invoke(tokenResponse);
            }
            catch (Exception ex)
            {
                onError?.Invoke(statusCode, $"Không parse được login response: {ex.Message}");
            }
        }
    }

    private static string BuildHttpErrorMessage(int statusCode, string transportError, string body)
    {
        List<string> parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(transportError))
            parts.Add(transportError);

        if (!string.IsNullOrWhiteSpace(body))
            parts.Add(body);

        if (statusCode == 401)
            parts.Add("401 Unauthorized — kiểm tra username/password hoặc Bearer token.");

        return string.Join(" | ", parts);
    }
}