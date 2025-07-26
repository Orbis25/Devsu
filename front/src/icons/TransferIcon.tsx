const TransferIcon = (props: React.SVGProps<SVGSVGElement>) => (
  <svg
    xmlns="http://www.w3.org/2000/svg"
    width={24}
    height={24}
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth={2}
    strokeLinecap="round"
    strokeLinejoin="round"
    className="icon icon-tabler icons-tabler-outline icon-tabler-transfer"
    {...props}
  >
    <path d="M0 0h24v24H0z" stroke="none" />
    <path d="M20 10H4l5.5-6M4 14h16l-5.5 6" />
  </svg>
);

export default TransferIcon;
