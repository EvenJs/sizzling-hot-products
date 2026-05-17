import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { PeriodSizzleCard } from "../../components/PeriodSizzleCard";

describe("PeriodSizzleCard", () => {
  it("shows loading state", () => {
    render(<PeriodSizzleCard data={null} loading={true} error={null} />);
    expect(screen.getByText("Loading...")).toBeInTheDocument();
  });

  it("shows error message", () => {
    render(
      <PeriodSizzleCard
        data={null}
        loading={false}
        error="Something went wrong"
      />,
    );
    expect(screen.getByText("Something went wrong")).toBeInTheDocument();
  });

  it("shows empty state when no data", () => {
    render(<PeriodSizzleCard data={null} loading={false} error={null} />);
    expect(screen.getByText("No data available.")).toBeInTheDocument();
  });

  it("renders product names when data is provided", () => {
    const data = {
      from: "2026-04-21",
      to: "2026-04-23",
      product: { id: "P1", name: "Hammer" },
    };
    render(<PeriodSizzleCard data={data} loading={false} error={null} />);
    expect(screen.getByText("Hammer")).toBeInTheDocument();
  });
});
