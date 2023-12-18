import { Outlet, useNavigate, useParams } from "react-router-dom";
import { useContext, useEffect, useState } from "react";
import Route from "./Route";
import routeData from "../../data/routes.json";
import { AppContext } from "../apps/AppContext";
import { Dropdown } from "flowbite-react";

export default function RouteIndex() {
  const navigate = useNavigate();
  const params = useParams();
  const app = useContext(AppContext)!;
  const [routes, setRoutes] = useState<Route[]>([]);
  const routeId = params["routeId"] ? parseInt(params["routeId"]) : undefined;
  console.log("routeId: ", routeId);
  useEffect(() => {
    setTimeout(() => {
      setRoutes(routeData.map((r) => ({ ...r, appId: app.id })));
    }, 100);
  }, []);
  const handleNewFolderClick = () => {};
  return (
    <div className="flex h-full">
      <div className="w-64 border-r p-1">
        <Dropdown size="xs" label="New">
          <Dropdown.Item onClick={() => navigate("/routes/new")}>
            Route
          </Dropdown.Item>
          <Dropdown.Item disabled onClick={handleNewFolderClick}>
            Folder
          </Dropdown.Item>
        </Dropdown>
        <ul className="mt-1">
          {routes.map((route) => {
            return (
              <li
                key={route.id}
                onClick={() => navigate(route.id!.toString())}
                className={route.id == routeId ? "bg-slate-100" : ""}
              >
                <p>{route.name}</p>
              </li>
            );
          })}
        </ul>
      </div>
      <div className="w-full">
        <Outlet key={routeId} />
      </div>
    </div>
  );
}
