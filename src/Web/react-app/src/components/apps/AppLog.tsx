import { useContext, useState } from "react";
import { AppContext } from "./AppContext";
import { useAuth0 } from "@auth0/auth0-react";
import ILog from "./Log";
import { SubmitHandler, useForm } from "react-hook-form";

type Inputs = {
  accessDate?: Date;
  routeId?: number;
};

export default function AppLogs() {
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();
  const [logs, setLogs] = useState<ILog[]>([]);

  const { register, handleSubmit } = useForm<Inputs>({
    defaultValues: {
      accessDate: new Date(),
    },
  });
  const onSubmit: SubmitHandler<Inputs> = async (data) => {
    const accessToken = await getAccessTokenSilently();
    var query = Object.keys(data)
      .map((key) => {
        if (data[key]) {
          return `${key}=${encodeURIComponent(data[key])}`;
        }
      })
      .join("&");
    const logs = (await (
      await fetch(`/api/apps/${app.id}/logs?${query}`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      })
    ).json()) as ILog[];
    setLogs(logs);
  };
  return (
    <>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div>
          <label>Access Date</label>
          <input type="date" {...register("accessDate")} />
        </div>
        <div>
          <label>Route</label>
          <select {...register("routeId")}></select>
        </div>
        <button type="submit">Filter</button>
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
