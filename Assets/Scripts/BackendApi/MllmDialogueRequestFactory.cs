using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Build request payload khớp AL-SERVICE Postman collection
/// (field names: current_shopping_list, time_rules, priorities, shifting_task, ...).
/// </summary>
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
        string eventCode = ResolveMapIntroEventCode(level);
        string details = eventDetails ?? (level == 2
            ? "Giới thiệu bản đồ các khu vực để bệnh nhân định hình lộ trình di chuyển tối ưu."
            : "Bệnh nhân vừa bước vào siêu thị");

        return BuildFromSession(session, eventCode, details, BuildMapLayoutContext(zones));
    }

    public static MllmGenerateDialogueRequest BuildReadShoppingList(
        GameSessionContext session,
        ListController listController,
        string eventDetails = null)
    {
        int level = session != null ? session.level : 1;
        string eventCode = ResolveReadShoppingListEventCode(level);
        string details = eventDetails ?? (level == 2
            ? "NPC đọc danh sách mua sắm cho bệnh nhân"
            : "Hiển thị bảng danh sách đồ cần mua");

        JObject context = BuildShoppingListContext(listController, level);
        return BuildFromSession(session, eventCode, details, context);
    }

    public static MllmGenerateDialogueRequest BuildRulesExplanation(
        GameSessionContext session,
        GameTimer gameTimer = null,
        string eventDetails = null)
    {
        int level = session != null ? session.level : 1;
        string eventCode = ResolveRulesEventCode(level);
        string details = eventDetails ?? (level == 2
            ? "Phổ biến luật chơi. Nhấn mạnh số lần được phép mở danh sách và thời gian mỗi lần."
            : "Giải thích luật chơi và thời gian cho bệnh nhân");

        return BuildFromSession(session, eventCode, details, BuildRulesContext(level, gameTimer));
    }

    public static MllmGenerateDialogueRequest BuildTimeUpAnnouncement(GameSessionContext session, GameTimer gameTimer)
    {
        int level = session != null ? session.level : 2;
        string eventCode = level >= 3
            ? MllmEventCodes.Lvl3TimeUpAnnouncement
            : MllmEventCodes.Lvl2TimeUpAnnouncement;

        return BuildFromSession(
            session,
            eventCode,
            level == 2 ? "Hết thời gian mua sắm" : "Thời gian đi chợ đã hết. Đưa ra lựa chọn khách quan.",
            BuildTimeUpContext(gameTimer));
    }

    public static MllmGenerateDialogueRequest BuildCheckoutCheck(
        GameSessionContext session,
        CartManager cartManager,
        PaymentSummary paymentSummary,
        bool hiddenTaskCollected = false,
        string hiddenTargetItem = "Sữa tươi",
        string consequenceIfMissed = "Mấy đứa nhỏ ở nhà")
    {
        int level = session != null ? session.level : 2;
        string eventCode = level >= 3
            ? MllmEventCodes.Lvl3CheckoutCheck
            : MllmEventCodes.Lvl2CheckoutCheck;

        return BuildFromSession(
            session,
            eventCode,
            "Kiểm tra giỏ hàng trước khi thanh toán",
            BuildCheckoutContext(cartManager, paymentSummary, hiddenTaskCollected, hiddenTargetItem, consequenceIfMissed));
    }

    public static MllmGenerateDialogueRequest BuildLvl2PrioritySetup(GameSessionContext session, DataManager dataManager)
    {
        int level = session != null ? session.level : 2;
        string eventCode = level >= 3
            ? MllmEventCodes.Lvl3PrioritySetup
            : MllmEventCodes.Lvl2PrioritySetup;

        return BuildFromSession(
            session,
            eventCode,
            level == 2 ? "Thiết lập thứ tự ưu tiên ghé các quầy" : "Giao các yêu cầu ưu tiên mua sắm",
            BuildPriorityContext(level));
    }

    public static MllmGenerateDialogueRequest BuildLvl2HiddenTaskSetup(
        GameSessionContext session,
        ListController listController)
    {
        int level = session != null ? session.level : 2;
        string eventCode = level >= 3
            ? MllmEventCodes.Lvl3HiddenTaskSetup
            : MllmEventCodes.Lvl2HiddenTaskSetup;

        return BuildFromSession(
            session,
            eventCode,
            level == 2 ? "NPC giới thiệu nhiệm vụ ẩn Level 2" : "Trích dẫn lời nhắn từ người thân để giao nhiệm vụ ẩn (Prospective Memory).",
            BuildHiddenTaskContext(level, listController));
    }

    public static MllmGenerateDialogueRequest BuildOutOfStockWarning(GameSessionContext session)
    {
        int level = session != null ? session.level : 2;
        string eventCode = level >= 3
            ? MllmEventCodes.Lvl3OutOfStockWarning
            : MllmEventCodes.Lvl2OutOfStockWarning;

        return BuildFromSession(
            session,
            eventCode,
            "Dặn dò quy tắc thay thế sản phẩm dự phòng khi hết hàng.",
            BuildOutOfStockContext(level));
    }

    public static MllmGenerateDialogueRequest BuildShiftingOrder(
        GameSessionContext session,
        string oldItem,
        string newItem,
        string reason = null,
        int newQuantity = 1)
    {
        int level = session != null ? session.level : 2;
        string eventCode = level >= 3
            ? MllmEventCodes.Lvl3ShiftingOrder
            : MllmEventCodes.Lvl2ShiftingOrder;

        string details = level == 2
            ? $"Danh sách đổi từ {oldItem} sang {newItem}"
            : "Người thân gọi điện yêu cầu đổi món.";
        return BuildFromSession(
            session,
            eventCode,
            details,
            BuildShiftingOrderContext(level, oldItem, newItem, reason, newQuantity));
    }

    public static MllmGenerateDialogueRequest BuildFlashSaleDistraction(
        GameSessionContext session,
        string itemName,
        string discountInfo = null,
        int discountPercentage = 20,
        string oldItem = null,
        int newQuantity = 1)
    {
        int level = session != null ? session.level : 2;
        string eventCode = level >= 3
            ? MllmEventCodes.Lvl3FlashSaleDistraction
            : MllmEventCodes.Lvl2FlashSaleDistraction;

        return BuildFromSession(
            session,
            eventCode,
            level == 2
                ? "Sự kiện flash sale / giảm giá làm thay đổi danh sách"
                : "Thông báo giảm giá có điều kiện thời gian để bẫy chức năng Inhibition.",
            BuildFlashSaleContext(itemName, discountInfo, discountPercentage, oldItem, newQuantity));
    }

    public static MllmGenerateDialogueRequest BuildNpcDistraction(GameSessionContext session)
    {
        int level = session != null ? session.level : 2;
        string eventCode = level >= 3
            ? MllmEventCodes.Lvl3NpcDistraction
            : MllmEventCodes.Lvl2NpcDistraction;

        return BuildFromSession(
            session,
            eventCode,
            "Kích hoạt hội thoại cho các NPC tiếp thị tại quầy.",
            BuildNpcDistractionContext());
    }

    // ── context_data builders (Postman field names) ─────────────────────────

    public static JObject BuildMapLayoutContext(IEnumerable<MapZoneInfo> zones)
    {
        JArray layout = new JArray();
        if (zones != null)
        {
            foreach (MapZoneInfo zone in zones)
            {
                layout.Add(new JObject
                {
                    ["zone_name"] = zone.zoneName ?? string.Empty,
                    ["relative_position"] = zone.relativePosition ?? string.Empty
                });
            }
        }

        return new JObject { ["map_layout"] = layout };
    }

    /// <summary>
    /// Postman: total_items_target, total_budget_vnd, list_rules, shopping_list[{item_name, quantity, unit, unit_price_vnd}]
    /// </summary>
    public static JObject BuildShoppingListContext(ListController listController, int level = 1)
    {
        JArray items = new JArray();
        if (listController != null && listController.choicedItems != null)
        {
            foreach (GameObject item in listController.choicedItems)
            {
                if (item == null) continue;

                string displayName = item.transform.Find("Name")?.GetComponent<TMPro.TextMeshProUGUI>()?.text
                                     ?? item.name;
                string quantityText = item.transform.Find("Quantity")?.GetComponent<TMPro.TextMeshProUGUI>()?.text
                                      ?? "1";
                int quantity = ParsePositiveInt(quantityText, 1);

                items.Add(new JObject
                {
                    ["item_name"] = displayName,
                    ["quantity"] = quantity.ToString(CultureInfo.InvariantCulture),
                    ["unit"] = InferUnit(displayName),
                    ["unit_price_vnd"] = ResolveUnitPriceVnd(displayName)
                });
            }
        }

        bool isDynamic = level >= 2;
        int budget = level >= 3 ? 1500000 : level >= 2 ? 1000000 : 100000;

        return new JObject
        {
            ["list_view_count"] = listController != null ? listController.GetClickNumber() : 0,
            ["total_items_target"] = items.Count,
            ["total_budget_vnd"] = budget,
            ["list_rules"] = new JObject
            {
                ["is_dynamic"] = isDynamic,
                ["possible_mid_game_changes"] = isDynamic
            },
            ["shopping_list"] = items
        };
    }

    /// <summary>
    /// Postman: time_rules, ui_rules, (lvl1) physical_task
    /// </summary>
    public static JObject BuildRulesContext(int level, GameTimer gameTimer = null)
    {
        int limitSeconds = gameTimer != null
            ? Mathf.RoundToInt(gameTimer.limitSeconds)
            : level >= 3 ? 600 : level >= 2 ? 480 : 300;

        bool strictTimer = level >= 2;
        bool canReopen = level < 3;
        int maxReopen = level >= 3 ? 0 : level >= 2 ? 2 : 3;

        JObject context = new JObject
        {
            ["scene_name"] = SceneManager.GetActiveScene().name,
            ["time_rules"] = new JObject
            {
                ["limit_seconds"] = limitSeconds,
                ["is_strict_timer"] = strictTimer
            },
            ["ui_rules"] = new JObject
            {
                ["list_hidden"] = true,
                ["can_reopen_list"] = canReopen,
                ["max_reopen_times"] = maxReopen
            }
        };

        if (level >= 2)
        {
            ((JObject)context["ui_rules"])["display_duration_per_open_seconds"] = 32;
        }

        if (level <= 1)
        {
            context["physical_task"] = new JObject
            {
                ["target_object"] = "chai nước suối mẫu",
                ["location"] = "trên chiếc bàn ngay trước mặt"
            };
        }

        return context;
    }

    /// <summary>
    /// Postman: booth_priority + interaction_range + priorities[{trigger_condition, required_action, allow_time_delay}]
    /// </summary>
    public static JObject BuildPriorityContext(int level = 2)
    {
        JArray boothPriority = new JArray();
        if (level >= 3)
        {
            boothPriority.Add(new JObject { ["booth_name"] = "Đồ tươi sống", ["priority_index"] = 1 });
            boothPriority.Add(new JObject { ["booth_name"] = "Rau củ quả", ["priority_index"] = 2 });
            boothPriority.Add(new JObject { ["booth_name"] = "Đồ khô", ["priority_index"] = 3 });
            boothPriority.Add(new JObject { ["booth_name"] = "Đồ uống", ["priority_index"] = 4 });
            boothPriority.Add(new JObject { ["booth_name"] = "Đồ gia dụng", ["priority_index"] = 5 });
        }
        else
        {
            boothPriority.Add(new JObject { ["booth_name"] = "Quầy trái cây", ["priority_index"] = 1 });
            boothPriority.Add(new JObject { ["booth_name"] = "Quầy nước uống", ["priority_index"] = 2 });
            boothPriority.Add(new JObject { ["booth_name"] = "Quầy bánh kẹo", ["priority_index"] = 3 });
        }

        JArray priorities = new JArray
        {
            new JObject
            {
                ["trigger_condition"] = "Có nhiều sự lựa chọn giảm giá",
                ["required_action"] = "Chọn món giảm sâu nhất",
                ["allow_time_delay"] = true
            }
        };

        if (level >= 3)
        {
            priorities.Add(new JObject
            {
                ["trigger_condition"] = "Ngân sách sắp hết",
                ["required_action"] = "Bỏ món không thiết yếu trước",
                ["allow_time_delay"] = false
            });
        }

        return new JObject
        {
            ["booth_priority"] = boothPriority,
            ["interaction_range"] = 10,
            ["priorities"] = priorities
        };
    }

    /// <summary>
    /// Postman: shopping_list + list_view_count + hidden_task_hint + caller_info + prospective_memory_task
    /// </summary>
    public static JObject BuildHiddenTaskContext(int level = 2, ListController listController = null)
    {
        JObject context = BuildShoppingListContext(listController, level);
        context["hidden_task_hint"] = level >= 3
            ? "Nhớ mua thêm vitamin D khi đi qua quầy gia dụng nếu thấy giảm giá"
            : "Bệnh nhân cần hoàn thành nhiệm vụ ẩn trong phiên mua sắm";

        if (level >= 3)
        {
            context["caller_info"] = new JObject
            {
                ["relation"] = "Con trai",
                ["name"] = "Minh"
            };
            context["prospective_memory_task"] = new JObject
            {
                ["trigger_zone"] = "Đồ gia dụng",
                ["target_item"] = "Vitamin D",
                ["quantity"] = 1,
                ["unit"] = "hộp",
                ["condition"] = "Giảm giá từ 15% trở lên"
            };
            return context;
        }

        context["caller_info"] = new JObject
        {
            ["relation"] = "Con gái",
            ["name"] = "Lan"
        };
        context["prospective_memory_task"] = new JObject
        {
            ["trigger_zone"] = "Đồ gia dụng",
            ["target_item"] = "Sữa tươi",
            ["quantity"] = 1,
            ["unit"] = "hộp",
            ["condition"] = "Giảm giá 25%"
        };
        return context;
    }

    /// <summary>
    /// Postman: out_of_stock_list[{item_name, acceptable_substitutes}]
    /// </summary>
    public static JObject BuildOutOfStockContext(int level = 2)
    {
        JArray list = new JArray
        {
            new JObject
            {
                ["item_name"] = level >= 3 ? "Thịt gà" : "Thịt bò",
                ["acceptable_substitutes"] = level >= 3
                    ? new JArray("Thịt heo", "Đậu hũ")
                    : new JArray("Thịt heo", "Thịt gà")
            }
        };

        if (level >= 3)
        {
            list.Add(new JObject
            {
                ["item_name"] = "Cá thu",
                ["acceptable_substitutes"] = new JArray("Cá basa", "Tôm")
            });
        }

        return new JObject { ["out_of_stock_list"] = list };
    }

    /// <summary>
    /// Postman: old_item + new_item + new_quantity + caller_info + shifting_task{old_item, new_item, reason}
    /// </summary>
    public static JObject BuildShiftingOrderContext(
        int level,
        string oldItem,
        string newItem,
        string reason = null,
        int newQuantity = 1)
    {
        string relation = level >= 3 ? "Con trai" : "Con gái";
        string name = level >= 3 ? "Minh" : "Lan";
        string resolvedReason = reason;
        if (string.IsNullOrWhiteSpace(resolvedReason))
        {
            resolvedReason = level >= 3
                ? "Nhà còn nhiều mì, cần phở thay thế"
                : "Ở nhà phát hiện vẫn còn 2 chai nước mắm chưa dùng";
        }

        return new JObject
        {
            ["old_item"] = oldItem ?? string.Empty,
            ["new_item"] = newItem ?? string.Empty,
            ["new_quantity"] = Mathf.Max(1, newQuantity),
            ["caller_info"] = new JObject
            {
                ["relation"] = relation,
                ["name"] = name
            },
            ["shifting_task"] = new JObject
            {
                ["old_item"] = oldItem ?? string.Empty,
                ["new_item"] = newItem ?? string.Empty,
                ["reason"] = resolvedReason
            }
        };
    }

    /// <summary>
    /// Postman: old_item + new_item + new_quantity + discount_info + discount_items[{item_name, offers[{discount_percentage, condition_instruction}]}]
    /// </summary>
    public static JObject BuildFlashSaleContext(
        string itemName,
        string discountInfo = null,
        int discountPercentage = 20,
        string oldItem = null,
        int newQuantity = 1)
    {
        string name = string.IsNullOrWhiteSpace(itemName) ? "Bánh quy bơ" : itemName;
        int pct = Mathf.Clamp(discountPercentage, 1, 90);

        // Parse "Đã giảm giá X N phần trăm" nếu có
        if (!string.IsNullOrWhiteSpace(discountInfo))
        {
            int parsed = ExtractDiscountPercentage(discountInfo);
            if (parsed > 0)
                pct = parsed;
        }

        return new JObject
        {
            ["old_item"] = string.IsNullOrWhiteSpace(oldItem) ? "Bánh quy" : oldItem,
            ["new_item"] = name,
            ["new_quantity"] = Mathf.Max(1, newQuantity),
            ["discount_info"] = discountInfo ?? $"Giảm {pct}% nếu quay lại sau 5 phút",
            ["discount_items"] = new JArray
            {
                new JObject
                {
                    ["item_name"] = name,
                    ["offers"] = new JArray
                    {
                        new JObject
                        {
                            ["discount_percentage"] = Mathf.Max(10, pct / 2),
                            ["condition_instruction"] = "Mua ngay bây giờ"
                        },
                        new JObject
                        {
                            ["discount_percentage"] = pct,
                            ["condition_instruction"] = "Quay lại mua sau 5 phút"
                        }
                    }
                }
            }
        };
    }

    /// <summary>
    /// Postman: solicitor_npcs[{npc_id, offered_item, selling_point, promotion}]
    /// </summary>
    public static JObject BuildNpcDistractionContext()
    {
        return new JObject
        {
            ["solicitor_npcs"] = new JArray
            {
                new JObject
                {
                    ["npc_id"] = "STAFF_01",
                    ["offered_item"] = "Sầu riêng Ri6",
                    ["selling_point"] = "Mới về, cơm vàng hạt lép",
                    ["promotion"] = "Giảm ngay 20%"
                },
                new JObject
                {
                    ["npc_id"] = "STAFF_02",
                    ["offered_item"] = "Bia nhập khẩu",
                    ["selling_point"] = "Vừa ướp lạnh xong",
                    ["promotion"] = "Mua 1 lốc tặng 1 ly thủy tinh"
                },
                new JObject
                {
                    ["npc_id"] = "STAFF_03",
                    ["offered_item"] = "Tôm hùm xanh",
                    ["selling_point"] = "Tôm còn bơi, bao tươi sống",
                    ["promotion"] = "Tặng kèm sốt chấm"
                }
            }
        };
    }

    /// <summary>
    /// Postman: decision_rule.available_options[{option_code, description}]
    /// (+ limit_seconds từ timer khi có)
    /// </summary>
    public static JObject BuildTimeUpContext(GameTimer gameTimer)
    {
        JObject context = new JObject
        {
            ["decision_rule"] = new JObject
            {
                ["available_options"] = new JArray
                {
                    new JObject
                    {
                        ["option_code"] = "continue_shopping",
                        ["description"] = "Tiếp tục tìm mua (bị phạt điểm thời gian)"
                    },
                    new JObject
                    {
                        ["option_code"] = "stop_and_checkout",
                        ["description"] = "Ra thanh toán ngay (chấp nhận bỏ món)"
                    }
                }
            }
        };

        if (gameTimer != null)
            context["limit_seconds"] = Mathf.RoundToInt(gameTimer.limitSeconds);

        return context;
    }

    /// <summary>
    /// Postman: hidden_task_results[{target_item, is_collected, consequence_if_missed}]
    /// </summary>
    public static JObject BuildCheckoutContext(
        CartManager cartManager,
        PaymentSummary paymentSummary,
        bool hiddenTaskCollected,
        string hiddenTargetItem,
        string consequenceIfMissed)
    {
        JObject context = new JObject
        {
            ["hidden_task_results"] = new JArray
            {
                new JObject
                {
                    ["target_item"] = hiddenTargetItem ?? "Sữa tươi",
                    ["is_collected"] = hiddenTaskCollected,
                    ["consequence_if_missed"] = consequenceIfMissed ?? string.Empty
                }
            }
        };

        if (cartManager != null)
        {
            // Bổ sung runtime numbers (backend có thể dùng; Postman example chủ yếu hidden_task_results)
            context["total_paid"] = cartManager.TotalPaid;
            context["bill_item_count"] = cartManager.bill != null ? cartManager.bill.Count : 0;
            context["shopping_bill"] = BuildShoppingBill(cartManager);

            if (!hiddenTaskCollected && cartManager.bill != null && !string.IsNullOrWhiteSpace(hiddenTargetItem))
            {
                foreach (var entry in cartManager.bill.Values)
                {
                    if (entry != null &&
                        !string.IsNullOrEmpty(entry.itemName) &&
                        entry.itemName.IndexOf(hiddenTargetItem, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ((JObject)((JArray)context["hidden_task_results"])[0])["is_collected"] = true;
                        break;
                    }
                }
            }
        }

        if (paymentSummary != null)
        {
            context["payment_result"] = new JObject
            {
                ["required_amount"] = paymentSummary.requiredAmount,
                ["paid_amount"] = paymentSummary.paidAmount,
                ["difference_amount"] = paymentSummary.differenceAmount,
                ["result_code"] = paymentSummary.resultCode,
                ["note"] = paymentSummary.note
            };
            context["player_wallet_start"] = BuildWalletArray(paymentSummary.startingWallet);
            context["player_cash_submitted"] = BuildWalletArray(paymentSummary.submittedBills);
        }

        return context;
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

    // ── helpers ─────────────────────────────────────────────────────────────

    private static string ResolveMapIntroEventCode(int level)
    {
        if (level >= 3) return MllmEventCodes.Lvl3MapIntro;
        if (level >= 2) return MllmEventCodes.Lvl2MapIntro;
        return MllmEventCodes.Lvl1MapIntro;
    }

    private static string ResolveReadShoppingListEventCode(int level)
    {
        if (level >= 3) return MllmEventCodes.Lvl3ReadShoppingList;
        if (level >= 2) return MllmEventCodes.Lvl2ReadShoppingList;
        return MllmEventCodes.Lvl1ReadShoppingList;
    }

    private static string ResolveRulesEventCode(int level)
    {
        if (level >= 3) return MllmEventCodes.Lvl3RulesExplanation;
        if (level >= 2) return MllmEventCodes.Lvl2RulesExplanation;
        return MllmEventCodes.Lvl1RulesExplanation;
    }

    private static int ParsePositiveInt(string text, int fallback)
    {
        if (int.TryParse(text, out int value) && value > 0)
            return value;
        return fallback;
    }

    private static string InferUnit(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            return "cái";

        string lower = itemName.ToLowerInvariant();
        if (lower.Contains("bó") || lower.Contains("rau")) return "bó";
        if (lower.Contains("hộp") || lower.Contains("sữa")) return "hộp";
        if (lower.Contains("chai") || lower.Contains("nước mắm") || lower.Contains("dầu")) return "chai";
        if (lower.Contains("kg") || lower.Contains("thịt") || lower.Contains("gạo")) return "kg";
        if (lower.Contains("khay")) return "khay";
        if (lower.Contains("thùng") || lower.Contains("bia")) return "thùng";
        if (lower.Contains("lốc")) return "lốc";
        if (lower.Contains("con") || lower.Contains("cá")) return "con";
        return "cái";
    }

    private static int ResolveUnitPriceVnd(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            return 0;

        SelectableItem[] selectables = Object.FindObjectsOfType<SelectableItem>(true);
        foreach (SelectableItem selectable in selectables)
        {
            if (selectable == null || string.IsNullOrEmpty(selectable.itemName))
                continue;

            if (string.Equals(selectable.itemName, itemName, System.StringComparison.OrdinalIgnoreCase) ||
                itemName.IndexOf(selectable.itemName, System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                selectable.itemName.IndexOf(itemName, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return selectable.price;
            }
        }

        return 0;
    }

    private static int ExtractDiscountPercentage(string discountInfo)
    {
        if (string.IsNullOrWhiteSpace(discountInfo))
            return 0;

        // "Đã giảm giá Bánh quy 20 phần trăm" / "Giảm 20%"
        System.Text.RegularExpressions.Match match =
            System.Text.RegularExpressions.Regex.Match(discountInfo, @"(\d+)\s*(%|phần trăm|phan tram)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int pct))
            return pct;

        return 0;
    }

    private static JArray BuildShoppingBill(CartManager cartManager)
    {
        JArray billItems = new JArray();
        if (cartManager == null || cartManager.bill == null)
            return billItems;

        foreach (BillEntry entry in cartManager.bill.Values)
        {
            if (entry == null)
                continue;

            billItems.Add(new JObject
            {
                ["item_name"] = entry.itemName ?? string.Empty,
                ["quantity"] = entry.quantity,
                ["unit_price_vnd"] = entry.price,
                ["subtotal_vnd"] = entry.price * entry.quantity
            });
        }

        return billItems;
    }

    private static JArray BuildWalletArray(IEnumerable<WalletBillSnapshot> walletSnapshots)
    {
        JArray wallet = new JArray();
        if (walletSnapshots == null)
            return wallet;

        foreach (WalletBillSnapshot snapshot in walletSnapshots)
        {
            if (snapshot == null)
                continue;

            wallet.Add(new JObject
            {
                ["denomination"] = snapshot.denomination,
                ["count"] = snapshot.count,
                ["subtotal"] = snapshot.subtotal
            });
        }

        return wallet;
    }
}
