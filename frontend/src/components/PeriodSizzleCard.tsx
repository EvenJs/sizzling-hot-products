import type { PeriodResult } from "../types/api";
import { formatDate } from "../utils/dateFormatter";

interface Props {
  data: PeriodResult | null | undefined;
  loading: boolean;
  error: string | null;
}

export function PeriodSizzleCard({ data, loading, error }: Props) {
  if (loading) return <div className="text-gray-500 p-4">Loading...</div>;
  if (error) return <div className="text-red-500 p-4">{error}</div>;
  if (!data) return <div className="text-gray-500 p-4">No data available.</div>;
  return (
    <div className="bg-green-50 border border-green-200 rounded-lg p-6">
      <p className="text-sm text-gray-500">
        {formatDate(data.from)} – {formatDate(data.to)}
      </p>
      <h2 className="text-xl font-bold text-green-700 mt-1">
        {data.product.name}
      </h2>
    </div>
  );
}
