import XIcon from "../icons/IconX";

function Modal({
  title,
  description,
  children,
  open,
  onClose,
}: {
  title: string;
  description: string;
  children?: React.ReactNode;
  open?: boolean;
  onClose?: () => void;
}) {
  if (!open) return null;
  return (
    <div className="modal">
      <div className="modal-card">
        <h2>{title}</h2>
        <p>{description}</p>
        <button onClick={onClose} className="modal-btn">
          <XIcon />
        </button>
        <div>{children}</div>
      </div>
    </div>
  );
}

export default Modal;
