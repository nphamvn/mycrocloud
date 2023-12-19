import { Outlet, useLocation, useNavigate, useParams } from "react-router-dom";
import { useContext, useEffect, useState } from "react";
import Route from "./Route";
import routeData from "../../data/routes.json";
import { AppContext } from "../apps/AppContext";
import { Dropdown } from "flowbite-react";
import RouteCreateUpdate from "./RouteCreateUpdate";

export default function RouteIndex() {
  const app = useContext(AppContext)!;
  const [routes, setRoutes] = useState<Route[]>([]);
  const [methods, setMethods] = useState<string[]>([]);
  const navigate = useNavigate();
  const { pathname } = useLocation();
  const childPath = pathname.split("/")[4];
  const params = useParams();
  const routeId = params["routeId"] ? parseInt(params["routeId"]) : undefined;
  useEffect(() => {
    setTimeout(() => {
      setRoutes(routeData.map((r) => ({ ...r, appId: app.id })));
    }, 100);
    setTimeout(() => {
      setMethods(["GET", "POST"]);
    }, 100);
  }, []);
  useEffect(() => {
    const childPath = pathname.split("/")[4];
    if (childPath === "new") {
      setRoutes([{ id: 0, name: "", method: "GET", path: "" }, ...routes]);
    }
  }, [pathname]);
  const handleNewFolderClick = () => {};
  return (
    <div className="flex h-full">
      <div className="w-64 border-r p-1">
        <Dropdown size="xs" label="New">
          <Dropdown.Item onClick={() => navigate("new")}>Route</Dropdown.Item>
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
                <RouteItem route={route} />
              </li>
            );
          })}
        </ul>
      </div>
      <div className="w-full">
        {childPath === "new" || routeId !== undefined ? (
          <RouteCreateUpdate
            key={routeId}
            routeId={routeId}
            methods={methods}
          />
        ) : (
          <div>
            Click New button to create new route or click route to edit.
          </div>
        )}
      </div>
    </div>
  );
}
function RouteItem({ route }: { route: Route }) {
  const methodTextColors = new Map<string, string>([
    ["GET", "text-sky-400"],
    ["POST", "text-orange-400"],
  ]);
  return (
    <div className="flex items-center" style={{ cursor: "pointer" }}>
      <span
        className={`me-1 font-semibold ${methodTextColors.get(
          route.method.toUpperCase(),
        )}`}
        style={{ fontSize: "0.65rem" }}
      >
        {route.method}
      </span>
      <p className="text-sm">{route.name}</p>
    </div>
  );
}
