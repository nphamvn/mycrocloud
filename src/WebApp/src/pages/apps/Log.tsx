import { useContext, useEffect, useState } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { SubmitHandler, useForm } from "react-hook-form";
import { AppContext } from ".";
import { IRouteLog } from "../routes";

type Inputs = {
  accessDateFrom?: string;
  accessDateTo?: string;
  routeIds: number[];
};

export default function AppLogs() {
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();
  const [logs, setLogs] = useState<IRouteLog[]>([]);
  const { register, handleSubmit, setValue } = useForm<Inputs>();
  const onSubmit: SubmitHandler<Inputs> = async (data) => {
    const accessToken = await getAccessTokenSilently();
    let query = "";
    const conditions = [];
    if (data.accessDateFrom) {
      conditions.push(`accessDateFrom=${data.accessDateFrom}`);
    }
    if (data.accessDateTo) {
      conditions.push(`accessDateTo=${data.accessDateTo}`);
    }
    if (data.routeIds.length > 0) {
      data.routeIds.forEach((id) => {
        conditions.push(`routeIds=${id}`);
      });
    }
    if (conditions.length > 0) {
      query += `&${conditions.join("&")}`;
    }
    console.log(query);
    const logs = (await (
      await fetch(`/api/apps/${app.id}/logs?${query}`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      })
    ).json()) as IRouteLog[];
    setLogs(logs);
  };
  const [routeIdsValue, setRouteIdsValue] = useState("");
  useEffect(() => {
    if (routeIdsValue) {
      setValue(
        "routeIds",
        routeIdsValue.split(",").map((id) => parseInt(id)),
      );
    } else {
      setValue("routeIds", []);
    }
  }, [routeIdsValue]);

  return (
    <>
      <form onSubmit={handleSubmit(onSubmit)} className="border p-2">
        <div>
          <label className="me-2">Access Date</label>
          <input type="date" {...register("accessDateFrom")} />
          <span>~</span>
          <input type="date" {...register("accessDateTo")} />
        </div>
        <div>
          <label className="me-2">Route</label>
          <input
            type="text"
            value={routeIdsValue}
            onChange={(e) => setRouteIdsValue(e.target.value)}
            className="border border-gray-200"
          />
        </div>
        <div className="flex">
          <button
            type="submit"
            className="ms-auto bg-primary px-2 py-0.5 text-white"
          >
            Filter
          </button>
        </div>
      </form>
      <hr />
      <table className="w-full">
        <thead>
          <tr>
            <th>Timestamp</th>
            <th>Remote Address</th>
            <th>Route Id</th>
            <th>Method</th>
            <th>Path</th>
            <th>Status Code</th>
          </tr>
        </thead>
        <tbody>
          {logs.map((l) => (
            <tr key={l.id} className="border">
              <td>{new Date(l.timestamp).toLocaleString()}</td>
              <td>{l.remoteAddress}</td>
              <td className="tooltip">
                {l.routeId || "-"}
                <span className="tooltiptext">
                  {l.routeName || "NOT FOUND"}
                </span>
              </td>
              <td>{l.method}</td>
              <td>{l.path}</td>
              <td>{l.statusCode}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}
