   M S B A M L   `   `   `  ????O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5       TradeStation.Option7   1TradeStation.Option.Views.CombinedFutureTradeView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      q mInfragisticsWPF4.DataManager.v15.1, Version=15.1.20151.2251, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb? ?InfragisticsWPF4.Controls.Editors.XamComboEditor.v15.1, Version=15.1.20151.2251, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cbw	 sInfragisticsWPF4.DataVisualization.v15.1, Version=15.1.20151.2251, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb|
 xInfragisticsWPF4.Controls.Grids.XamGrid.v15.1, Version=15.1.20151.2251, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cbe aInfragisticsWPF4.v15.1, Version=15.1.20151.2251, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb? ?InfragisticsWPF4.Controls.Editors.XamMaskedInput.v15.1, Version=15.1.20151.2251, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb? ~InfragisticsWPF4.Controls.Charts.XamDataChart.v15.1, Version=15.1.20151.2251, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb9ig$http://schemas.infragistics.com/xaml   	 
    5      <sys&clr-namespace:System;assembly=mscorlib   	 
    5        ? 	Resources  5
       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $ ..\\Resources.xaml??6%   %    5      .??5      ? .+?5   
   ??  ?? Orientation$ Vertical=?6   .+?5      ?? $ 
Horizontal=?6   .+?5      ? $??5,5,5,5q?6    ? RowDefinitions 5      ?? 5      $??* ?6(   ?? 5      $??* ?6(   ?? 5      $??* ?6(    ? ColumnDefinitions 5      ?? 5      $
??Auto ?6+   ?? 5      $
??Auto ?6+   ?? 5      $
??Auto ?6+   ?? 5       $
??Auto ?6+   .+?5#      ?? $??0??6    $??0??6-   $??Center=?6=   $??市场??6X   2  ,Infragistics.Controls.Editors.XamComboEditor  5$      $??0??6(   $??1??65   $	??100??6E   ?  ?Infragistics.Controls.Editors.ComboEditorBase`2[[Infragistics.Controls.Editors.ComboEditorItem, InfragisticsWPF4.Controls.Editors.XamComboEditor.v15.1, Version=15.1.20151.2251, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb],[Infragistics.Controls.Editors.ComboEditorItemControl, InfragisticsWPF4.Controls.Editors.XamComboEditor.v15.1, Version=15.1.20151.2251, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb]]   ItemsSource 5%   (   ?? *OptionMarkets+   SelectedItem 5&   (   ?? *SelectedOptionMarket+.  (System.Windows.Interactivity.Interaction   Triggers 5'      /  )System.Windows.Interactivity.EventTrigger  5(      	   	EventName$	 SelectionChanged??6-   .  (System.Windows.Interactivity.TriggerBase
   Actions.
 5)   "   6  0System.Windows.Interactivity.InvokeCommandAction     Command 68   ?? *OptionMarketChangedCommand+5*      5,      ?? 5.      $??0??6    $??2??6-   $??Center=?6=   $??	买组合??6X     5/      $??0??6(   $??3??65   $	??100??6E    50   (   ?? *Menubar.CombiNos+ 51   (   ?? *SelectedBuyCombineNo+52      ?? 54      $??1??6    $??0??6-   $??Center=?6=   $??标的名称??6X     55      $??1??6(   $??1??65   $	??100??6E    56   (   ?? *UnderlyingSecurityNames+ 57   (   ?? *SelectedUnderlyingSecurity+ 58        59      $	 SelectionChanged??6-   .
 5:   "      68   ?? *" UnderlyingSecurityChangedCommand+5;      5=      ?? 5?      $??1??6    $??2??6-   $??Center=?6=   $??	卖组合??6X     5@      $??1??6(   $??3??65   $	??100??6E    5A   (   ?? *Menubar.CombiNos+ 5B   (   ?? *SelectedSellCombineNo+5C      ?? 5E      $??2??6    $??0??6-   $??Center=?6=   $??期权月份??6X     5F      $??2??6(   $??1??65   $	??100??6E    5G   (   ?? *DeliveryMonthList+ 5H   (   ?? *DeliveryMonth+ 5I        5J      $	 SelectionChanged??6-   .
 5K   "      68   ?? *%#SelectedDeliveryMonthChangedCommand+5L      5N      5O      5P      ?? 5R      ???	#FFABABAB$??1q?6-   ._?5S      ?? $
??Auto=?6   $	??345??6B   .??5T      ) 
 #Infragistics.Controls.Grids.XamGrid     AutoGenerateColumns .     RowHover$	 Row=?5W   !      
ActiveItem 5T   !   ?? *SelectedOptionItem+   ItemsSource 5V   !   ?? *DisplayOptions+   FilteringSettings 5X      3 
 -Infragistics.Controls.Grids.FilteringSettings  5Y         AllowFiltering$ 
FilterMenu=?63      SelectionSettings 5[      3	 
 -Infragistics.Controls.Grids.SelectionSettings	  5\       	  CellClickAction$ 	SelectRow=?63    	  CellSelection$
 None=?6O      Columns 5^      0
 
 *Infragistics.Controls.Grids.TemplateColumn
  5_      , 
 &Infragistics.Controls.Grids.ColumnBase
   Key$ ExercisePrice??60      
HeaderText$ 	行权价??5`   0   ( 
 "Infragistics.Controls.Grids.Column   IsFilterable .  0 
 *Infragistics.Controls.Grids.EditableColumn   
IsReadOnly .  
  ItemTemplate 5c   "   ?? 5d   &   .R?5e   *   ?? $ 
Horizontal=?65   .+?5f   .   ?? ??5g   2   `? 5h   6    %  !DigitFormatStringWithTagConverter `? 	Converter# [  6C   .1?5i   :   ??  ?? Path$ ExercisePrice?6B   ?? 5j   :   $( "CallOptionSecurityInfo.PriceDigits?6B   ?? 5k   :   $ OptionExRightSymbol?6B   5l   7   5n   /   5o   +   5p   '   5r      
  5s      $. (CallOptionSecurityInfo.Quotation.BidVol1??60   $ 买量??5t   0    .   .  5w   "   ?? 5x   &   .R?5y   *   ?? $ 
Horizontal=?65   .+?5z   .   ?? ??5{   2   `? 5|   6     GreaterValueConverter# [ 6C    `? ConverterParameter$
 true??6v   .1?5}   :   ?? $. (CallOptionSecurityInfo.Quotation.BidVol1?6B    ?? IsAsync . 6r   ?? 5~   :   $- 'PutOptionSecurityInfo.Quotation.AskVol1?6B    . 6q   5   7   5?   /   5?   +   5?   '   5?      
  5?      $- 'CallOptionSecurityInfo.Quotation.BidPx1??60   $ 买价??5?   0    .   .  5?   "   ?? 5?   &   .R?5?   *   ?? $ 
Horizontal=?65   .+?5?   .   ?? 
???  ????5?   2   `? 5?   6      CombinedFuturePriceConverter# [ 6C   .1?5?   :   ?? $- 'CallOptionSecurityInfo.Quotation.BidPx1?6B   ?? 5?   :   $, &PutOptionSecurityInfo.Quotation.AskPx1?6B   ?? 5?   :   $ ExercisePrice?6B   ?? 5?   :   $( "CallOptionSecurityInfo.PriceDigits?6B   5?   7   5?   /   5?   +   5?   '   5?      
  5?      $, &PutOptionSecurityInfo.Quotation.AskPx1??60   $ 卖价??5?   1    .   .  5?   "   ?? 5?   &   .R?5?   *   ?? $ 
Horizontal=?65   .+?5?   .   ?? 
??? ? ???5?   2   `? 5?   6   # [ 6C   .1?5?   :   ?? $- 'CallOptionSecurityInfo.Quotation.AskPx1?6B   ?? 5?   :   $, &PutOptionSecurityInfo.Quotation.BidPx1?6B   ?? 5?   :   $ ExercisePrice?6B   ?? 5?   :   $' !PutOptionSecurityInfo.PriceDigits?6B   5?   7   5?   /   5?   +   5?   '   5?      
  5?      $- 'PutOptionSecurityInfo.Quotation.AskVol1??60   $ 卖量??5?   0    .   .  5?   "   ?? 5?   &   .R?5?   *   ?? $ 
Horizontal=?65   .+?5?   .   ?? ??5?   2   `? 5?   6   # [ 6C   $
 true??6v   .1?5?   :   ?? $. (CallOptionSecurityInfo.Quotation.AskVol1?6B    . 6r   ?? 5?   :   $- 'PutOptionSecurityInfo.Quotation.BidVol1?6B    . 6q   5?   7   5?   /   5?   +   5?   '   5?       5?        5?      $#	 SelectedRowsCollectionChanged??6-   .
 5?   "      68   ?? *" SelectedOptionItemChangedCommand+5?      5?      5?      5?      ?? 5?      $ 
Horizontal=?6   .+?5?      ? $??5,5,5,5q?6    5?      ?? 5?      $??* ?6(   ?? 5?      $??* ?6(   ?? 5?      $??* ?6(   ?? 5?      $??* ?6(    5?      ?? 5?      $??* ?6+   ?? 5?      $??* ?6+   .+?5?      ?? $??0??6    $??0??6-   $??Center=?5?        ?? TextAlignment$
 Left=?6;   $??60??5?       $??	行权价??5?       ? 5?      $??0??6   $??1??6(   $??Center=?68   .+?5?      ?? $	??120??6$   $??25??60   $
??Left=?5?   $   ??5?      `? 5?   "   # [  6/   .1?5?   &   ?? $&  SelectedOptionItem.ExercisePrice?6.   ?? 5?   &   $; 5SelectedOptionItem.CallOptionSecurityInfo.PriceDigits?6.   ?? 5?   &   $, &SelectedOptionItem.OptionExRightSymbol?6.   5?   #   5?      5?      ?? 5?      $??1??6    $??0??6-   $??Center=?5?       $
 Left=?6;   $??60??5?       $??张数??5?       3  -Infragistics.Controls.Editors.XamNumericInput  5?      $??1??6)   $??1??66   $
??Left=?6F   $??95??6a   2  ,Infragistics.Controls.Editors.XamMaskedInput    ClipMode$	  Raw=?5?   )   !   DataMode$	! Raw=?68   "   DisplayMode$	" Raw=?6G   .  (Infragistics.Controls.Editors.ValueInput#   InvalidValueBehavior$# RevertValue=?5?   )   $   	ValueType"$ ??5?   )   %   
PromptChar@  :Infragistics.Controls.Editors.Primitives.MaskCharConverter$%   5?   )   &   Mask$& 	nnnnnnnnn??5?   )   '   SpinButtonDisplayMode$' Always=?5?   )   (   Value( 5?   )   ?? *EntrustAmount+) ?? UpdateSourceTrigger$) PropertyChanged=?*   ValueConstraint* 5?      3  -Infragistics.Controls.Editors.ValueConstraint  5?      +   MinInclusive$+ 1??61   6D   5?      ?? 5?      $??2??6    $??0??6-   $??Center=?5?       $
 Left=?6;   $??60??5?       $??委托类型??5         5     $??2??6(   $??1??65   $??95??6E   $
??Left=?5  (   $??0,3,3,3q?5  (   ,   
IsEditable, .   5  (   ?? *!CombinedOptionEntrustPriceTypes+ 5  (   ?? *" SelectedCombinedOptiontPriceType+5     ?? 5	     $??3??6    $??0??6-   $??Center=?5
      $
 Left=?6;   $??60??5      $??追加ticks??5        5     $??3??6)   $??1??66   $
??Left=?6F   $??95??6a   $	  Raw=?5  )   $	! Raw=?68   $	" Raw=?6G   $# RevertValue=?5  )   "$ ??5  )   $%   5  )   $& nn??5  )   $' Always=?5  )   ( 5  )   ?? *EntrustTickCount+$) PropertyChanged=?5     5     ? 5     $??5,5,5,5q?6    5     ?? 5     $??25 ?6(   ?? 5     $??* ?6(   ?? 5     $??* ?6(   ?? 5     $??* ?6(    5     ?? 5     $??* ?6+   ?? 5      $??* ?6+   .+?5#     ?? $??1??6    $??0??6-   $??Center=?5$      $
 Left=?6;   $??60??5%      $??下单方法??5&        5'     $??1??6(   $??1??65   $??95??6E   $
??Left=?5(  (   $??0,3,3,3q?5)  (   , .   5+  (   ??   odpBasketTradeMethod- ?? Source#- [  5,  (   ?? *SelectedBasketTradeMethod+5-     5/     50     ?? 52     $ 
Horizontal=?6   .+?53     ?? $??25??6   $??75??6%   $??Center=?54     $??Center=?64   $??买入??6Q    55       56     $	 Click??6)   .
 57        64   ?? *CombinedFutureBuyCommand+58     5:     ?? 5;     $??25??6   $??75??6%   $??Center=?5<     $??Center=?64   $??卖出??6Q    5=       5>     $	 Click??6)   .
 5?        64   ?? *CombinedFutureSellCommand+5@     5B     5C     5D     5E     5F     