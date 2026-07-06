using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MllmDialogueRequestFactory
{
    public static MllmGenerateDialogueRequest Build(
        string citizenId,
        int level,
        string gamePhase,
        string eventCode,
        string eventDetails,
        JObject contextData = null,
        string imageBase64 = null)
    {
        return new MllmGenerateDialogueRequest
        {
            citizen_id = citizenId,
            level = level,
            game_phase = gamePhase,
            content = new MllmContentData
            {
                event_code = eventCode,
                event_details = eventDetails,
                context_data = contextData ?? new JObject()
            },
            image_base64 = imageBase64
        };
    }

    public static MllmGenerateDialogueRequest BuildFromSession(
        GameSessionContext session,
        string eventCode,
        string eventDetails,
        JObject contextData = null,
        string imageBase64 = null)
    {
        if (session == null)
        {
            Debug.LogWarning("[MllmDialogueRequestFactory] GameSessionContext null — dùng citizen_id rỗng.");
            return Build(string.Empty, 1, MllmGamePhases.PreGame, eventCode, eventDetails, contextData, imageBase64);
        }

        return Build(session.citizenId, session.level, session.gamePhase, eventCode, eventDetails, contextData, imageBase64);
    }

    public static MllmGenerateDialogueRequest BuildMapIntro(
        GameSessionContext session,
        IEnumerable<MapZoneInfo> zones,
        string eventDetails = null)
    {
        int level = session != null ? session.level : 1;
        string eventCode = level >= 2 ? MllmEventCodes.Lvl2MapIntro : MllmEventCodes.Lvl1MapIntro;
        string details = eventDetails ?? "Bệnh nhân vừa bước vào siêu thị";

        return BuildFromSession(session, eventCode, details, BuildMapLayoutContext(zones));
    }

    public static MllmGenerateDialogueRequest BuildReadShoppingList(
        GameSessionContext session,
        ListController listController,
        string eventDetails = null)
    {
        int level = session != null ? session.level : 1;
        string eventCode = level >= 2 ? MllmEventCodes.Lvl2ReadShoppingList : MllmEventCodes.Lvl1ReadShoppingList;
        string details = eventDetails ?? "NPC đọc danh sách mua sắm cho bệnh nhân";

        JObject context = BuildShoppingListContext(listController);
        return BuildFromSession(session, eventCode, details, context);
    }

    public static MllmGenerateDialogueRequest BuildRulesExplanation(
        GameSessionContext session,
        string eventDetails = null)
    {
        int level = session != null ? session.level : 1;
        string eventCode = level >= 2 ? MllmEventCodes.Lvl2RulesExplanation : MllmEventCodes.Lvl1RulesExplanation;
        string details = eventDetails ?? "NPC giải thích luật chơi và mục tiêu";

        return BuildFromSession(session, eventCode, details, BuildSceneContext());
    }

    public static MllmGenerateDialogueRequest BuildTimeUpAnnouncement(GameSessionContext session, GameTimer gameTimer)
    {
        JObject context = new JObject();
        if (gameTimer != null)
            context["limit_seconds"] = gameTimer.limitSeconds;

        return BuildFromSession(
            session,
            MllmEventCodes.Lvl2TimeUpAnnouncement,
            "Hết thời gian mua sắm",
            context);
    }

    public static MllmGenerateDialogueRequest BuildCheckoutCheck(GameSessionContext session, CartManager cartManager)
    {
        JObject context = new JObject();
        if (cartManager != null)
        {
            context["total_paid"] = cartManager.TotalPaid;
            context["bill_item_count"] = cartManager.bill != null ? cartManager.bill.Count : 0;
        }

        return BuildFromSession(
            session,
            MllmEventCodes.Lvl2CheckoutCheck,
            "Kiểm tra giỏ hàng trước khi thanh toán",
            context);
    }

    public static MllmGenerateDialogueRequest BuildLvl2PrioritySetup(GameSessionContext session, DataManager dataManager)
    {
        JObject context = new JObject();
        if (dataManager != null && dataManager.targets != null)
        {
            JArray booths = new JArray();
            string[] boothNames = { "Quầy trái cây", "Quầy nước uống", "Quầy bánh kẹo" };
            for (int i = 0; i < dataManager.targets.Length; i++)
            {
                if (dataManager.targets[i] == null)
                    continue;

                booths.Add(new JObject
                {
                    ["booth_name"] = i < boothNames.Length ? boothNames[i] : dataManager.targets[i].name,
                    ["priority_index"] = i + 1
                });
            }

            context["booth_priority"] = booths;
            context["interaction_range"] = dataManager.interactionRange;
        }

        return BuildFromSession(
            session,
            MllmEventCodes.Lvl2PrioritySetup,
            "Thiết lập thứ tự ưu tiên ghé các quầy",
            context);
    }

    public static MllmGenerateDialogueRequest BuildLvl2HiddenTaskSetup(
        GameSessionContext session,
        ListController listController)
    {
        JObject context = BuildShoppingListContext(listController);
        context["hidden_task_hint"] = "Bệnh nhân cần hoàn thành nhiệm vụ ẩn trong phiên mua sắm";

        return BuildFromSession(
            session,
            MllmEventCodes.Lvl2HiddenTaskSetup,
            "NPC giới thiệu nhiệm vụ ẩn Level 2",
            context);
    }

    public static JObject BuildMapLayoutContext(IEnumerable<MapZoneInfo> zones)
    {
        JArray layout = new JArray();
        if (zones != null)
        {
            foreach (MapZoneInfo zone in zones)
            {
                layout.Add(new JObject
                {
                    ["zone_name"] = zone.zoneName,
                    ["relative_position"] = zone.relativePosition
                });
            }
        }

        return new JObject { ["map_layout"] = layout };
    }

    public static JObject BuildShoppingListContext(ListController listController)
    {
        JArray items = new JArray();
        if (listController != null && listController.choicedItems != null)
        {
            foreach (GameObject item in listController.choicedItems)
            {
                if (item == null) continue;

                string name = item.name;
                string quantity = item.transform.Find("Quantity")?.GetComponent<TMPro.TextMeshProUGUI>()?.text ?? "1";
                string displayName = item.transform.Find("Name")?.GetComponent<TMPro.TextMeshProUGUI>()?.text ?? name;

                items.Add(new JObject
                {
                    ["item_name"] = displayName,
                    ["quantity"] = quantity
                });
            }
        }

        return new JObject
        {
            ["shopping_list"] = items,
            ["list_view_count"] = listController != null ? listController.GetClickNumber() : 0
        };
    }

    public static JObject BuildSceneContext()
    {
        return new JObject
        {
            ["scene_name"] = SceneManager.GetActiveScene().name
        };
    }

    public static JObject ParseContextJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new JObject();

        try
        {
            return JObject.Parse(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[MllmDialogueRequestFactory] context JSON không hợp lệ: {ex.Message}");
            return new JObject();
        }
    }
}