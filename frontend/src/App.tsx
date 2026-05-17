import { useSizzlingHot } from "./hooks/useSizzlingHot";
import { useDateRange } from "./hooks/useDateRange";
import { DailySizzleTable } from "./components/DailySizzleTable";
import { PeriodSizzleCard } from "./components/PeriodSizzleCard";
import DateRangePicker from "./components/DateRangePicker";

function App() {
  const { from, to, validationError, handleFromChange, handleToChange } =
    useDateRange("2026-04-21", "2026-04-23");
  const { data, loading, error } = useSizzlingHot(
    validationError ? "" : from,
    validationError ? "" : to,
  );

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-green-600 text-white p-6">
        <h1 className="text-2xl font-bold">Top Products Dashboard</h1>
      </header>
      <main className="max-w-4xl mx-auto p-6 space-y-8">
        <div>
          <div className="flex gap-4 items-end">
            <DateRangePicker
              label="From"
              value={from}
              onChange={handleFromChange}
            />
            <DateRangePicker label="To" value={to} onChange={handleToChange} />
          </div>
          {validationError && (
            <p className="text-red-500 text-sm mt-2">{validationError}</p>
          )}
        </div>
        <section>
          <h2 className="text-lg font-semibold mb-3">Daily Breakdown</h2>
          <DailySizzleTable
            data={data?.daily}
            loading={loading}
            error={error}
          />
        </section>
        <section>
          <h2 className="text-lg font-semibold mb-3">Period Winner</h2>
          <PeriodSizzleCard
            data={data?.period}
            loading={loading}
            error={error}
          />
        </section>
      </main>
    </div>
  );
}

export default App;
