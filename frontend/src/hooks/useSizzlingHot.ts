import { useEffect, useState } from "react";
import type { DailyResult, PeriodResult } from "../types/api";

export function useSizzlingHot() {
  const [daily, setDaily] = useState<DailyResult[]>([]);
  const [period, setPeriod] = useState<PeriodResult | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const baseUrl = import.meta.env.VITE_API_URL;

        const [dailyRes, periodRes] = await Promise.all([
          fetch(`${baseUrl}/api/products/sizzling-hot/daily`),
          fetch(`${baseUrl}/api/products/sizzling-hot/period`),
        ]);

        if (!dailyRes.ok || !periodRes.ok) {
          throw new Error('Failed to fetch data');
        }

        const dailyData = await dailyRes.json();
        const periodData = await periodRes.json();

        setDaily(dailyData);
        setPeriod(periodData);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
      } finally {
        setLoading(false)
      }
    }
    fetchData();
  }, [])

  return { daily, period, loading, error };
}