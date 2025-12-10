import { Link } from "react-router-dom";

export const Landing: React.FC = () => {
    return (
        <div style={{display: 'grid', gridTemplateRows: 'auto 1fr', width: '100vw', gridTemplateColumns: 'repeat(3, 1fr)'}}>
            <div style={{gridColumn: '2/3', textAlign: 'center'}}>
                <p>Landing</p>
                <Link to="/auth">Start</Link>
            </div>
        </div>
    );
};