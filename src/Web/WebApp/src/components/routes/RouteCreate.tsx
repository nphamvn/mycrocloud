import { useAuth0 } from "@auth0/auth0-react";
import RouteCreateUpdate from "./RouteCreateUpdate";
import { RouteCreateUpdateInputs } from "./RouteCreateUpdateInputs";
import { useContext, useEffect } from "react";
import { AppContext } from "../apps/AppContext";
import { toast } from "react-toastify";
import Route from "./Route";
import { useRoutesContext } from "./RoutesContext";

const newRoute: Route = {
  id: 0,
  name: "",
  path: "",
  method: "GET",
  responseType: "static",
  responseStatusCode: 200,
  responseHeaders: [{ name: "content-type", value: "text/plain" }],
  responseBodyLanguage: "text/plain",
  responseBody: "",
  requireAuthorization: false,
  status: "active",
  useDynamicResponse: false,
};

export default function RouteCreate() {
  const app = useContext(AppContext)!;
  const { dispatch } = useRoutesContext();
  const { getAccessTokenSilently } = useAuth0();
  const onSubmit = async (data: RouteCreateUpdateInputs) => {
    const accessToken = await getAccessTokenSilently();
    const res = await fetch(`/api/apps/${app.id}/routes`, {
      method: "POST",
      headers: {
        "content-type": "application/json",
        Authorization: `Bearer ${accessToken}`,
      },
      body: JSON.stringify(data),
    });
    if (res.ok) {
      toast("Route created");
    }
  };
  useEffect(() => {
    console.log("adding new route");
    dispatch({ type: "ADD_ROUTE", payload: newRoute });
    //dispatch({ type: "SET_ACTIVE_ROUTE", payload: newRoute });
  }, []);
  return <RouteCreateUpdate route={newRoute} onSubmit={onSubmit} />;
}
