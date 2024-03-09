import { Outlet, useNavigate, useParams, useMatch } from "react-router-dom";
import { useContext, useEffect, useReducer } from "react";
import IRoute from "./Route";
import { AppContext } from "../apps";
import { useAuth0 } from "@auth0/auth0-react";
import {
  RoutesContext,
  routesReducer,
  useRoutesContext,
} from "./RoutesContext";

export default function RouteIndex() {
  const app = useContext(AppContext)!;
  const [state, dispatch] = useReducer(routesReducer, {
    routes: [],
    activeRoute: undefined,
  });
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();
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
      ).json()) as IRoute[];
      dispatch({ type: "SET_ROUTES", payload: routes });
    };
    getRoutes();
  }, []);

  const newRouteActive = useMatch("/apps/:appId/routes/new");
  const editRouteActive = useMatch("/apps/:appId/routes/:routeId");
  const logPageActive = useMatch("/apps/:appId/routes/:routeId/logs");

  return (
    <RoutesContext.Provider value={{ state, dispatch }}>
      <div className="flex h-full">
        <div className="w-48 border-r p-1">
          <button
            type="button"
            onClick={() => {
              navigate("new");
            }}
            className="mt-1 w-full bg-primary py-1 text-white disabled:opacity-50"
            disabled={newRouteActive !== null}
          >
            New
          </button>
          <RouteList />
        </div>
        <div className="flex-1">
          {newRouteActive || editRouteActive || logPageActive ? (
            <div className="">
              <Outlet key={routeId} />
            </div>
          ) : (
            <div className="flex h-screen items-center justify-center">
              Click New button to create new route or click route to edit.
            </div>
          )}
        </div>
      </div>
    </RoutesContext.Provider>
  );
}

function RouteList() {
  const {
    state: { routes, activeRoute },
  } = useRoutesContext();
  const navigate = useNavigate();
  return (
    <ul className="mt-1">
      {routes.map((route) => {
        return (
          <li
            key={route.id}
            onClick={() => {
              navigate(route.id!.toString());
            }}
            className={route.id == activeRoute?.id ? "bg-slate-100" : ""}
          >
            <RouteItem route={route} />
          </li>
        );
      })}
    </ul>
  );
}
function RouteItem({ route }: { route: IRoute }) {
  const methodTextColors = new Map<string, string>([
    ["GET", "text-sky-500"],
    ["POST", "text-orange-500"],
    ["PUT", "text-green-500"],
    ["DELETE", "text-red-500"],
    ["PATCH", "text-yellow-500"],
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
      {route.status === "Blocked" && (
        <small className="ms-auto text-sm text-red-600">Blocked</small>
      )}
    </div>
  );
}
