import { Link, useLocation } from "react-router-dom";
import UserIcon from "../icons/UserIcon";
import WalletIcon from "../icons/WalletIcon";
import TransferIcon from "../icons/TransferIcon";
import ReportIcon from "../icons/ReportIcon";

function Sidebar() {
  const pathname = useLocation().pathname;
  return (
    <aside className="sidebar">
      <nav>
        <ul>
          <li className={pathname === "/clients" ? "active" : ""}>
            <UserIcon className="icon" />
            <Link to="/clients">Clientes</Link>
          </li>
          <li className={pathname === "/accounts" ? "active" : ""}>
            <WalletIcon className="icon" />
            <Link to="/accounts">Cuentas</Link>
          </li>
          <li className={pathname === "/movements" ? "active" : ""}>
            <TransferIcon className="icon" />
            <Link to="/movements">Movimientos</Link>
          </li>
          <li className={pathname === "/reports" ? "active" : ""}>
            <ReportIcon className="icon" />
            <Link to="/reports">Reportes</Link>
          </li>
        </ul>
      </nav>
    </aside>
  );
}

export default Sidebar;
