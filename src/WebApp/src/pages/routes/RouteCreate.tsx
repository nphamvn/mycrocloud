import { useAuth0 } from "@auth0/auth0-react";
import RouteCreateUpdate from "./RouteCreateUpdate";
import { RouteCreateUpdateInputs } from "./RouteCreateUpdateInputs";
import { useContext } from "react";
import { AppContext } from "../apps";
import { toast } from "react-toastify";
import IRoute from "./Route";
import { useRoutesContext } from "./RoutesContext";

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
      const newRoute = (await res.json()) as IRoute;
      dispatch({ type: "ADD_ROUTE", payload: newRoute });
      toast("Route created");
    }
  };
  return <RouteCreateUpdate onSubmit={onSubmit} />;
}
