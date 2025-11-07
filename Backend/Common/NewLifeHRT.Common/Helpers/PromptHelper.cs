using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.Helpers
{
    public static class PromptHelper
    {
        public static string BuildRefillPrompt(object input)
        {
            var jsonInput = JsonSerializer.Serialize(input);
            return $@"
                   You are a pharmacy refill calculator.

                   Your job is to read a product name, a free-text dosing protocol, a quantity dispensed, and a start_date. Compute:
                   days_supply (integer, ceil/round up),
                   refill_date (ISO date = start_date + days_supply).

                   Parsing & computation rules
                   Skip non-medication supplies. If the product name includes any of: needle, syringe, alcohol, swab, bacteriostatic, water, bandage, cotton, sharps, lancet, gauze → return status=""skip_supply"". 
                   Refillcalcuation
                   Reject non-actionable protocols. If protocol is like “Use as directed” or begins with “Dissolve …” with no dose/frequency you can compute → status=""unparsable"". 
                   Refillcalcuation
                   Dose + Unit extraction (priority order)
                   Injectables: Inject … <number> ml|cc|units (normalize cc→ml, units→unit). If text says “Inject … 3 times weekly” with a number but missing unit, infer ml when bottle/reconstitution context exists. 
                   Refillcalcuation
                   Solids (capsule/tablet/troche/ODT): phrases like “take 1 capsule”, “take 1/2 tablet”, “take 1 troche”, default amount 1 if unit known but number omitted. Normalize unit names. 
                   Refillcalcuation
                   Fallback: any <number> <ml|capsule|tablet|troche|odt|unit|click>, ignoring numbers that belong to “mix/reconstitute”. 
                   Refillcalcuation
                   Frequency detection
                   Daily: map once daily/daily/nightly/qhs/qpm → 1; BID→2; TID→3; QID→4; every N hours → 24/N; every N days → 1/N; M/W/F → 3/7.
                   Weekly: map once/1x weekly → 1; twice/2x weekly → 2; three/3x weekly → 3; <N> times per week, <N> days per week, or explicit weekday lists (count unique days).
                   If a solid dose has no explicit frequency but mentions AM & PM (e.g., “in morning and afternoon”, “AM & PM”), infer 2/day. 
                   Refillcalcuation
                   Bottle size / pack size / counts
                   Injectables total volume (ml):
                   Prefer protocol reconstitution: mix/reconstitute (each vial) with <X> ml.
                   Else infer from product name like “10 ml”.
                   Vial count: from “dispense X vials/bottles” or from line quantity. 
                   Refillcalcuation
                   Solids total units:
                   If product name has pack size: “<NN> CT/ct” → total units = NN × number of packs (quantity).
                   Else, if unit is solid and no CT, use quantity as unit count. Normalize unit from product name if needed.
                   Topicals with clicks: multiply clicks_per_bottle × bottle_count when stated. 
                   Refillcalcuation
                   Computation
                   Injectables (ml):
                   If weekly frequency: weekly_ml = dose_ml × freq_per_week; days = ceil(total_ml / weekly_ml × 7).
                   If daily frequency: daily_ml = dose_ml × freq_per_day; days = ceil(total_ml / daily_ml).
                   Solids (capsules/tablets/troches/ODT/units):
                   Weekly: days = ceil(total_units / (dose_units × freq_per_week) × 7)
                   Daily: days = ceil(total_units / (dose_units × freq_per_day))
                   Topicals (clicks): identical pattern using clicks as units.
                   Always CEILING the day count to avoid underestimation. refill_date = start_date + days.
                   Weekly phrasing examples the model must understand: once/weekly, twice weekly, “3x weekly”, “N days per week”, named weekdays (“every Monday and Thursday”). Daily examples: once daily, BID/TID/QID, every N hours, every N days, every other day, nightly/bedtime. 
                   Algorithm for HRT Protocol Pars…
                   Outputs

                   Return ONLY a single, valid JSON object. Do NOT include any explanation, backticks, markdown, or extra characters.
                   The JSON must match this exact schema:
                   {{
                        ""status"": ""ok|skip_supply|unparsable|error"",
                        ""days_supply"": 0,
                        ""refill_date"": ""YYYY-MM-DD"",
                        ""details"": {{
                        ""dose_amount"": 0,
                        ""dose_unit"": """",
                        ""frequency_per_day"": null,
                        ""frequency_per_week"": null,
                        ""bottle_size_ml"": null,
                        ""vial_count"": null,
                        ""units_count"": null,
                        ""clicks_per_bottle"": null,
                        ""assumptions"": []
                        }}
                    }}

                   Strictness & safety:
                   If required fields to compute are missing after best-effort parsing (no dose, no frequency, or no bottle/pack count), set status=""unparsable"" with a short reason in details.assumptions.
                   Do not hallucinate quantities; only infer where the rules above explicitly allow it.
                   Dates are calendar dates (no time); treat start_date as day-0.

                   CRITICAL: You MUST return ONLY raw JSON without any additional text, explanations, or markdown formatting.
                   Do NOT wrap the JSON in backticks, quotes, or any other characters.
                   The response must begin with '{{' and end with '}}' with no leading or trailing characters.

                   Input:
                   {jsonInput}
                   ";
        }
    }
}
