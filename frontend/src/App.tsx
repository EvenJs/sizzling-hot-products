import { DailySizzleTable } from "./components/DailySizzleTable";
import { PeriodSizzleCard } from "./components/PeriodSizzleCard";
import { useSizzlingHot } from "./hooks/useSizzlingHot";

function App() {
  const { daily, period, loading, error } = useSizzlingHot();

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-green-600 text-white p-6">
        <h1 className="text-2xl font-bold">Top Products Dashboard</h1>
      </header>
      <main className="max-w-4xl mx-auto p-6 space-y-8">
        <section>
          <h2 className="text-lg font-semibold mb-3">Daily Breakdown</h2>
          <DailySizzleTable data={daily} loading={loading} error={error} />
        </section>
        <section>
          <h2 className="text-lg font-semibold mb-3">3-Day Winner</h2>
          <PeriodSizzleCard data={period} loading={loading} error={error} />
        </section>
      </main>
    </div>
  );
}

export default App;
