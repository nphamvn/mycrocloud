import { Outlet, useNavigate, useParams } from "react-router-dom";
import { useContext, useEffect, useState } from "react";
import Route from "./Route";
import routeData from "../../data/routes.json";
import { AppContext } from "../apps/AppContext";
import { Button } from "flowbite-react";

export default function RouteIndex() {
  const location = useNavigate();
  const params = useParams();
  const app = useContext(AppContext)!;
  const [routes, setRoutes] = useState<Route[]>([]);
  const routeId = params["routeId"] ? parseInt(params["routeId"]) : undefined;

  useEffect(() => {
    setTimeout(() => {
      setRoutes(routeData.map((r) => ({ ...r, appId: app.id })));
    }, 100);
  }, []);
  return (
    <div className="">
      <h1>Routes</h1>
      <Button size="xs" onClick={() => location("new")}>
        New
      </Button>
      <div>
        <ul>
          {routes.map((route) => {
            return (
              <li
                key={route.id}
                onClick={() => location(route.id!.toString())}
                className={route.id == routeId ? "bg-slate-100" : ""}
              >
                <p>{route.name}</p>
              </li>
            );
          })}
        </ul>
        <Outlet key={routeId} />
      </div>
    </div>
  );
}
