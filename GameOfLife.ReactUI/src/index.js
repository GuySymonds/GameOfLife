import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';

class Square extends React.Component {
    renderThis(c) {
        if (c === 1) {
            return '#';
        }
    }

    render() {
        const myStyle = this.renderThis(this.props.value) ? { backgroundColor: 'black' } : null;

        return (
            <span style={myStyle} className="square"></span>
        );
    }
}

class ToggleButton extends React.Component {
    render() {
        return <button onClick=''>Pause</button>
    };
}

class Board extends React.Component {
    constructor(props) {
        super(props)
        this.state = {
            squares: null,
            id: 'e6829659-e497-461d-8313-2993b9a3d9e8'
        };
    }

    renderSquare(row, column) {
        
        return <Square value={this.state.squares === null ? '' : this.state.squares[row][column]} />;
    }

    renderRow(row, columns) {
        var cells = [];
        for (let index = 0; index < columns; index++) {
            cells.push(this.renderSquare(row, index));
        }
        return <div className="board-row">{cells}</div>
    }

    renderGrid(rowCount, columns) {
        var rows = [];
        for (let index = 0; index < rowCount; index++) {
            rows.push(this.renderRow(index, columns, index * columns));
        }
        return rows
    }

    fetchData() {
        fetch('https://localhost:5001/api/games/' + this.state.id + '/next', {
            method: 'GET',
            headers: { 'Accept': 'application/json' }
        })
            .then((response) => response.json().then(data => {
                this.setState({ squares: data.cells });
                this.setState({ id: data.gameId });
                this.fetchData();
            }));
    }

    startNewGame() {
        fetch('https://localhost:5001/api/games/', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                width: 40,
                height: 18,
            })
        }).then((response) => response.json().then(data => {
            this.setState({ squares: data.cells });
            this.setState({ id: data.gameId });
            this.fetchData();
        }));
    }

    componentDidMount() {
        this.startNewGame();
        this.fetchData();
    }

    render() {
        return (
            <div>
                {this.renderGrid(18, 40)}
            </div>
        );
    }
}

class Game extends React.Component {

    render() {
        return (
            <div className="game">
                <div className="game-board">
                    <Board />
                </div>
                <div className="game-info">
                    <ToggleButton onClick={() => { this.handleClick() }} />
                </div>
            </div>
        );
    }
}

// ========================================
ReactDOM.render(
    <Game />,
    document.getElementById('root')
);
