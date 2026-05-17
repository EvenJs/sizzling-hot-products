import type { DailyResult } from "../types/api";
import { formatDate } from "../utils/dateFormatter";

interface Props {
  data: DailyResult[] | undefined;
  loading: boolean;
  error: string | null;
}

export function DailySizzleTable({ data, loading, error }: Props) {
  if (loading) return <div className="text-gray-500 p-4">Loading...</div>;
  if (error) return <div className="text-red-500 p-4">{error}</div>;
  if (!data || data.length === 0)
    return (
      <div className="text-gray-500 p-4">
        No products found for this period.
      </div>
    );

  return (
    <table className="w-full border-collapse">
      <thead>
        <tr className="bg-green-600 text-white">
          <th className="p-3 text-left">Date</th>
          <th className="p-3 text-left">Top Product</th>
        </tr>
      </thead>
      <tbody>
        {data.map((daily) => (
          <tr key={daily.date} className="border-b hover:bg-gray-50">
            <td className="p-3">{formatDate(daily.date)}</td>
            <td className="p-3">{daily.product.name}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
