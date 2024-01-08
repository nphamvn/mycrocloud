import { useParams } from "react-router-dom";
import RouteCreateUpdate from "./RouteCreateUpdate";

export default function RouteEdit() {
  const routeId = parseInt(useParams()["routeId"]!);
  return <RouteCreateUpdate routeId={routeId} />;
}
