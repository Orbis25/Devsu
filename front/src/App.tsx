import { Route, Routes } from "react-router-dom";
import Accounts from "./pages/accounts/Accounts";
import Users from "./pages/users/Users";
import Transactions from "./pages/transactions/Transactions";
import Reports from "./pages/reports/Reports";
import Sidebar from "./components/Sidebar";
import Header from "./components/Header";
import { ToastContainer } from "react-toastify";

function App() {
  return (
    <div>
      <Header />
      <div className="main-container">
        <Sidebar />
        <main className="main-content">
          <Routes>
            <Route path="/clients" element={<Users />} />
            <Route path="/accounts" element={<Accounts />} />
            <Route path="/movements" element={<Transactions />} />
            <Route path="/reports" element={<Reports />} />
          </Routes>
        </main>
        <ToastContainer/>
      </div>
    </div>
  );
}

export default App;
