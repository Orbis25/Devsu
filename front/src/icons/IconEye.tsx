const IconEye = (props: React.SVGProps<SVGSVGElement>) => (
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
    className="icon icon-tabler icons-tabler-outline icon-tabler-eye"
    {...props}
  >
    <path d="M0 0h24v24H0z" stroke="none" />
    <path d="M10 12a2 2 0 1 0 4 0 2 2 0 0 0-4 0" />
    <path d="M21 12q-3.6 6-9 6t-9-6q3.6-6 9-6t9 6" />
  </svg>
);

export default IconEye;
