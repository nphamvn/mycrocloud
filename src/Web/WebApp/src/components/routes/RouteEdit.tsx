import { Link, useNavigate, useParams } from "react-router-dom";
import RouteCreateUpdate from "./RouteCreateUpdate";
import { useAuth0 } from "@auth0/auth0-react";
import { useContext, useEffect, useState } from "react";
import { AppContext } from "../apps/AppContext";
import { toast } from "react-toastify";
import Route from "./Route";
import { RouteCreateUpdateInputs } from "./RouteCreateUpdateInputs";
import { useRoutesContext } from "./RoutesContext";

export default function RouteEdit() {
  const app = useContext(AppContext)!;
  const {
    state: { routes },
    dispatch,
  } = useRoutesContext();
  const { getAccessTokenSilently } = useAuth0();
  const routeId = parseInt(useParams()["routeId"]!);
  const navigate = useNavigate();
  const [route, setRoute] = useState<Route>();
  const handleCloneClick = async () => {
    const accessToken = await getAccessTokenSilently();
    const res = await fetch(`/api/apps/${app.id}/routes/${routeId}/clone`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
    const newRoute = (await res.json()) as Route;
    if (res.ok) {
      toast.success("Route cloned");
      dispatch({ type: "ADD_ROUTE", payload: newRoute });
    }
  };
  const handleDeleteClick = async () => {
    if (confirm("Are you sure want to delete this route?")) {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/apps/${app.id}/routes/${routeId}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      if (res.ok) {
        navigate("../");
      }
    }
  };
  useEffect(() => {
    dispatch({
      type: "SET_ACTIVE_ROUTE",
      payload: routes.find((r) => r.id === routeId)!,
    });
    const getRoute = async () => {
      const accessToken = await getAccessTokenSilently();
      const route = (await (
        await fetch(`/api/apps/${app.id}/routes/${routeId}`, {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        })
      ).json()) as Route;
      setRoute(route);
    };
    getRoute();
  }, []);
  const onSubmit = async (data: RouteCreateUpdateInputs) => {
    const accessToken = await getAccessTokenSilently();
    const res = await fetch(`/api/apps/${app.id}/routes/${routeId}`, {
      method: "PUT",
      headers: {
        "content-type": "application/json",
        Authorization: `Bearer ${accessToken}`,
      },
      body: JSON.stringify(data),
    });
    if (res.ok) {
      toast("Route updated");
      route!.name = data.name;
      route!.method = data.method;
      dispatch({ type: "UPDATE_ROUTE", payload: route! });
    }
  };

  if (!route) {
    return null;
  }
  return (
    <>
      <div className="mb-1 border-b border-gray-200 dark:border-gray-700">
        <div className="flex items-end justify-end space-x-1 px-2">
          <Link to={"logs"}>Logs</Link>
          {/* <button className="border px-3 py-0.5">Test</button> */}
          {/* <button className="border px-3 py-0.5">Share</button>
                    <button className="border px-3 py-0.5">History</button>
                    <button className="border px-3 py-0.5">Settings</button> */}
          <button
            type="button"
            onClick={handleCloneClick}
            className="text-sm text-primary"
          >
            Clone
          </button>
          <button onClick={handleDeleteClick} className="text-sm text-red-600">
            Delete
          </button>
        </div>
      </div>
      <RouteCreateUpdate route={route} onSubmit={onSubmit} />
    </>
  );
}
