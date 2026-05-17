import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { DailySizzleTable } from "../../components/DailySizzleTable";

describe("DailySizzleTable", () => {
  it("shows loading state", () => {
    render(<DailySizzleTable data={[]} loading={true} error={null} />);
    expect(screen.getByText("Loading...")).toBeInTheDocument();
  });

  it("shows error message", () => {
    render(
      <DailySizzleTable
        data={[]}
        loading={false}
        error="Something went wrong"
      />,
    );
    expect(screen.getByText("Something went wrong")).toBeInTheDocument();
  });

  it("shows empty state when no data", () => {
    render(<DailySizzleTable data={[]} loading={false} error={null} />);
    expect(
      screen.getByText("No products found for this period."),
    ).toBeInTheDocument();
  });

  it("renders product names when data is provided", () => {
    const data = [
      { date: "2026-04-21", product: { id: "P1", name: "Hammer" } },
    ];
    render(<DailySizzleTable data={data} loading={false} error={null} />);
    expect(screen.getByText("Hammer")).toBeInTheDocument();
  });
});
