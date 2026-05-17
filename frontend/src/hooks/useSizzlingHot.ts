import { useEffect, useState } from "react";
import type { SizzlingHotResult } from "../types/api";

export function useSizzlingHot(from: string, to: string) {
  const [data, setData] = useState<SizzlingHotResult | null>(null)

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      if (!from || !to) return;
      setLoading(true);
      setError(null);

      try {
        const baseUrl = import.meta.env.VITE_API_URL;

        const response = await fetch(`${baseUrl}/api/products/sizzling-hot?from=${from}&to=${to}`);

        if (!response.ok) {
          throw new Error('Failed to fetch data');
        }

        const result = await response.json();
        setData(result);

      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
      } finally {
        setLoading(false)
      }
    }
    fetchData();
  }, [from, to])

  return { data, loading, error };
}