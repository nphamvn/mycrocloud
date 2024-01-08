import { useContext, useEffect, useState } from "react";
import { AppContext } from "./AppContext";
import { useAuth0 } from "@auth0/auth0-react";
import ILog from "./Log";
import { SubmitHandler, useForm } from "react-hook-form";

type Inputs = {
  accessDateFrom?: string;
  accessDateTo?: string;
  routeIds: number[];
};

export default function AppLogs() {
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();
  const [logs, setLogs] = useState<ILog[]>([]);
  const { register, handleSubmit, setValue } = useForm<Inputs>();
  const onSubmit: SubmitHandler<Inputs> = async (data) => {
    const accessToken = await getAccessTokenSilently();
    var query = Object.keys(data)
      .map((key) => {
        const value = data[key as keyof typeof data];
        if (value) {
          if (Array.isArray(value)) {
            return `${key}=${value.join(",")}`;
          }
          return `${key}=${encodeURIComponent(value as string)}`;
        }
      })
      .join("&");
    console.log(query);
    const logs = (await (
      await fetch(`/api/apps/${app.id}/logs?${query}`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      })
    ).json()) as ILog[];
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
      <form onSubmit={handleSubmit(onSubmit)} className="p-2">
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
            className="bg-primary ms-auto px-2 py-0.5 text-white"
          >
            Filter
          </button>
        </div>
      </form>
      <hr />
      <ul>
        {logs.map((l) => (
          <li key={l.id}>
            <div className="text-sm">
              {new Date(l.timestamp).toUTCString()} {l.method} {l.path}
            </div>
          </li>
        ))}
      </ul>
    </>
  );
}
