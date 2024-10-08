import { useAuth0 } from "@auth0/auth0-react";
import RouteCreateUpdate from "./CreateUpdateForm";
import { RouteCreateUpdateInputs } from "./CreateUpdateFormInputs";
import { useContext } from "react";
import { AppContext } from "../apps";
import { toast } from "react-toastify";
import IRoute from "./Route";
import { useRoutesContext } from "./Context";
import { useNavigate, useParams } from "react-router-dom";

export default function RouteCreate() {
  const navigate = useNavigate();
  const app = useContext(AppContext)!;
  const { dispatch } = useRoutesContext();

  const folderId = useParams()["folderId"];
  const { getAccessTokenSilently } = useAuth0();
  const onSubmit = async (data: RouteCreateUpdateInputs) => {
    const accessToken = await getAccessTokenSilently();
    const res = await fetch(`/api/apps/${app.id}/routes`, {
      method: "POST",
      headers: {
        "content-type": "application/json",
        Authorization: `Bearer ${accessToken}`,
      },
      body: JSON.stringify({
        ...data,
        folderId: folderId ? parseInt(folderId) : null,
      }),
    });
    if (res.ok) {
      const newRoute = (await res.json()) as IRoute;
      dispatch({ type: "ADD_ROUTE", payload: newRoute });
      toast("Route created");
      navigate(`../${newRoute.id}`);
    }
  };
  return <RouteCreateUpdate onSubmit={onSubmit} />;
}
