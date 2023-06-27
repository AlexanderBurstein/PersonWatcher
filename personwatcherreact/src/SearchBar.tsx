import React from 'react';
export interface Props {
    searchDeals: any,
    keyword?: string
  }
  
const BarStyle = {width:"20rem",background:"#F0F0F0", border:"none", padding:"0.5rem"};
class SearchBar extends React.Component<Props> {
    
  state = {
    searchTerm: ''
  }
  
  handleChange = (searchTerm: string) => {
    this.setState({ searchTerm }, () => {
      this.props.searchDeals(this.state.searchTerm)
    })
  }

  render() {
    return (
      <input 
      style={BarStyle}
      key="search-bar"
      defaultValue={this.props.keyword}
      placeholder="search..."
      onChange={(e) => this.handleChange(e.target.value)}
      />
    )
  }
}

export default SearchBar