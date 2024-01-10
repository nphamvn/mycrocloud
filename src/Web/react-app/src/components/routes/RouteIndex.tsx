import {
  Link,
  Outlet,
  useLocation,
  useNavigate,
  useParams,
} from "react-router-dom";
import { useContext, useEffect, useState } from "react";
import Route from "./Route";
import { AppContext } from "../apps/AppContext";
import { Dropdown } from "flowbite-react";
import { useAuth0 } from "@auth0/auth0-react";

export default function RouteIndex() {
  const app = useContext(AppContext)!;
  const [routes, setRoutes] = useState<Route[]>([]);
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();
  const { pathname } = useLocation();
  const childPath = pathname.split("/")[4];
  const tab = pathname.split("/")[5] || "edit";
  const params = useParams();
  const routeId = params["routeId"] ? parseInt(params["routeId"]) : undefined;
  useEffect(() => {
    const getRoutes = async () => {
      const accessToken = await getAccessTokenSilently();
      const routes = (await (
        await fetch(`/api/apps/${app.id}/routes`, {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        })
      ).json()) as Route[];
      setRoutes(routes);
    };
    getRoutes();
  }, []);
  useEffect(() => {
    const childPath = pathname.split("/")[4];
    if (childPath === "new") {
      setRoutes([
        {
          id: 0,
          name: "",
          method: "GET",
          path: "",
          responseType: "static",
          responseStatusCode: 200,
          responseHeaders: [],
          responseBodyLanguage: "json",
          responseBody: "",
        },
        ...routes,
      ]);
    }
  }, [pathname]);
  const handleNewFolderClick = () => {};
  const handleDeleteRouteClick = async (id: number) => {
    if (confirm("Are you sure want to delete this route?")) {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/apps/${app.id}/routes/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      if (res.ok) {
        navigate(`/apps/${app.id}/routes`);
      }
    }
  };
  return (
    <div className="flex h-full">
      <div className="w-48 border-r p-1">
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
      <div className="h-full flex-1">
        {childPath === "new" || routeId !== undefined ? (
          <div>
            {routeId !== undefined && (
              <div className="mb-1 border-b border-gray-200 dark:border-gray-700">
                <div className="flex">
                  <Link
                    to={`${routeId}`}
                    className={`border px-3 py-0.5 ${
                      tab === "edit"
                        ? "border-b-0 border-t-2 border-t-primary text-primary"
                        : ""
                    }`}
                  >
                    Edit
                  </Link>
                  <Link
                    to={`${routeId}/logs`}
                    className={`border px-3 py-0.5 ${
                      tab === "logs"
                        ? "border-b-0 border-t-2 border-t-primary text-primary"
                        : ""
                    }`}
                  >
                    Logs
                  </Link>
                  <div className="ms-auto">
                    {/* <button className="border px-3 py-0.5">Test</button> */}
                    {/* <button className="border px-3 py-0.5">Share</button>
                    <button className="border px-3 py-0.5">History</button>
                    <button className="border px-3 py-0.5">Settings</button> */}
                    <button
                      onClick={() => handleDeleteRouteClick(routeId)}
                      className="me-1 ms-auto text-sm text-red-600"
                    >
                      Delete
                    </button>
                  </div>
                </div>
              </div>
            )}
            <div className="h-full overflow-y-auto">
              <Outlet key={routeId} />
            </div>
          </div>
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
    ["PUT", "text-rose-400"],
    ["DELETE", "text-red-400"],
    ["PATCH", "text-yellow-400"],
  ]);
  return (
    <div className="flex items-center p-0.5" style={{ cursor: "pointer" }}>
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
